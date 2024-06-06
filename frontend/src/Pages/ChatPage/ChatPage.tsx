import adapter from 'webrtc-adapter'
import JanusJS from "janus-gateway"
import React, {useEffect, useState} from "react";
import {toast} from "react-toastify";
import {useAuth} from "../../Context/useAuth";
import {JanusServerWS, JanusTextRoomPlugin} from "../../Constants";
import {Chat} from "../../Components/Chat/Chat";
import {MessageModel} from "../../Components/Chat/MessageModel";
import {MessageTypesEnum} from "../../Components/Chat/MessageTypesEnum";
import {UserService} from "../../Services/UserService";
import {useNavigate} from "react-router-dom";
import {MovieService} from "../../Services/MovieService";
import {MovieDto} from "../../Models/MovieDto";
import {Player} from "../../Components/Player/Player";
import {VideoRoom} from "../../Components/VideoRoom/VideoRoom";

export const ChatPage = () => {

    enum stateOfPage {
        Connecting,
        RegisterUser,
        Success,
        WebRTCSupportError,
        CouldNotConnectToServer,
        AttachTextRoomError,
        RegisterUserError,
        RoomNotExist
    }

    const [movieList, setMovieList] = useState<MovieDto[]>([])

    useEffect(() => {
        MovieService.getAllMovie().then((r) => {
            if (r?.status == 200)
                setMovieList(r.data)
        })
    }, []);

    let myUsername: string | undefined = "";

    const [dp, setDp] = useState(JanusJS.useDefaultDependencies({adapter: adapter}))
    const [currentPageState, setCurrentPageState] = useState(stateOfPage.Connecting)
    let janus: JanusJS;

    const [isOwner, setIsOwner] = useState(false)
    const [whoIsOperator, setWhoIsOperator] = useState<string | undefined>()
    const [myId, setMyId] = useState<string>()

    const [syncedIsPlay, setSyncedIsPlay] = useState(false)
    const [syncedTime, setSyncedTime] = useState(0)
    const [movieId, setMovieId] = useState('')

    let [messageList, setMessageList] = useState<MessageModel[]>([])
    let [movieIdList, setMovieIdList] = useState<number[]>([])

    let [mapOfUsername, setMapOfUsername] = useState(new Map<string, string>())
    const [unpublish, setUnpublish] = useState(false)

    // @ts-ignore
    const [textRoom, setTextRoom] = useState<JanusJS.PluginHandle>();

    // @ts-ignore
    let textroom: JanusJS.PluginHandle;

    let isLoadingJanus = false;

    function InitJanus() {
        JanusJS.init({
            debug: "all",
            dependencies: dp,
            callback: function () {
                if (!JanusJS.isWebrtcSupported()) {
                    setCurrentPageState(stateOfPage.WebRTCSupportError)
                    return;
                }
                janus = new JanusJS(
                    {
                        server: JanusServerWS,
                        success: function () {
                            console.log("Janus create")
                            janus.attach(
                                {
                                    plugin: JanusTextRoomPlugin,
                                    opaqueId: "textroomtest-" + JanusJS.randomString(12),
                                    success: function (pluginHandle) {
                                        setTextRoom(pluginHandle)
                                        textroom = pluginHandle
                                        console.log("Plugin attached! (" + pluginHandle.getPlugin() + ", id=" + pluginHandle.getId() + ")");
                                        let body = {request: "setup"};
                                        pluginHandle.send({message: body})
                                    },
                                    error: function (error) {
                                        console.error("  -- Error attaching plugin...", error);
                                        setCurrentPageState(stateOfPage.AttachTextRoomError)
                                    },
                                    iceState: function (state) {
                                        JanusJS.log("ICE state changed to " + state);
                                    },
                                    webrtcState: function (on) {
                                        JanusJS.log("Janus says our WebRTC PeerConnection is " + (on ? "up" : "down") + " now");
                                    },
                                    onmessage: function (msg, jsep) {
                                        JanusJS.debug(" ::: Got a message :::", msg);
                                        if (msg["error"]) {
                                            toast.error(msg["error"]);
                                        }
                                        if (jsep) {
                                            textroom.createAnswer(
                                                {
                                                    jsep: jsep,
                                                    tracks: [
                                                        {type: 'data'}
                                                    ],
                                                    success: function (jsep: {}) {
                                                        JanusJS.debug("Got SDP!", jsep);
                                                        let body = {request: "ack"};
                                                        textroom.send({message: body, jsep: jsep});
                                                    },
                                                    error: function (error: any) {
                                                        JanusJS.error("WebRTC error:", error);
                                                        toast.error("WebRTC error... " + error.message);
                                                    }
                                                })
                                        }
                                    },
                                    ondataopen: function () {
                                        JanusJS.log("The DataChannel is available!");
                                        setCurrentPageState(stateOfPage.RegisterUser)
                                        registerUsername()
                                    },
                                    ondata: function (data: any) {
                                        JanusJS.debug("We got data from the DataChannel!", data);
                                        let json = JSON.parse(data);
                                        let what = json["textroom"]
                                        let from = ""
                                        let text = ""
                                        let name = ""
                                        let type = MessageTypesEnum.message
                                        if (what === "success") {
                                            if (json["participants"])
                                                for (let i of json["participants"]) {
                                                    let un = i["username"] as string
                                                    mapOfUsername.set(un, i["display"])
                                                    setMapOfUsername(mapOfUsername)
                                                }
                                            if (json["is_owner"])
                                                setIsOwner(json["is_owner"])
                                            if (json["is_operator"] == true)
                                                setWhoIsOperator(json["username"])
                                            else if (json["is_operator"] == false)
                                                setWhoIsOperator(undefined)
                                            if (json["operator_id"] != undefined)
                                                setWhoIsOperator(json["operator_id"])
                                            if (json["current_movie"])
                                                if (json["current_movie"] != movieId) {
                                                    setMovieId(json["current_movie"])
                                                }
                                            if (json["playlist"]) {
                                                movieIdList = json["playlist"]
                                                setMovieIdList(json["playlist"])
                                            }
                                        } else if (what === "error") {
                                            if (json["error_code"] == 417) setCurrentPageState(stateOfPage.RoomNotExist)
                                            if (json["error_code"] == 419) setCurrentPageState(stateOfPage.RegisterUserError)
                                            if (json["error_code"] == 420) window.location.reload()
                                        } else if (what === "new_operator") {
                                            setWhoIsOperator(json["username"])
                                        } else if (what === "sync") {
                                            if (json["is_playing"] != undefined) {
                                                setSyncedIsPlay(json["is_playing"])
                                            }
                                            if (json["time"] != undefined) {
                                                setSyncedTime(json["time"])
                                            }
                                            if (json["current_movie"])
                                                if (json["current_movie"] != movieId) {
                                                    setMovieId(json["current_movie"])
                                                }
                                            if (json["playlist"]) {
                                                movieIdList = json["playlist"]
                                                setMovieIdList(json["playlist"])
                                            }
                                        } else if (what === "destroyed") {
                                            toast.info("Kicked")
                                            leaveFromRoom()
                                        } else {
                                            if (what === "message") {
                                                console.log("message")
                                                type = MessageTypesEnum.message
                                                from = json["from"]
                                                text = json["text"]
                                            } else if (what === "leave") {
                                                console.log("leave")
                                                type = MessageTypesEnum.leave
                                                from = json["username"]
                                                name = mapOfUsername.get(from) as string
                                                mapOfUsername.delete(from)
                                                setMapOfUsername(mapOfUsername)
                                            } else if (what === "join") {
                                                console.log("join")
                                                type = MessageTypesEnum.join
                                                from = json["username"]
                                                if (json["display"] == myUsername) {
                                                    setMyId(from)
                                                }
                                                mapOfUsername.set(from, json["display"])
                                                setMapOfUsername(mapOfUsername)
                                                console.log("MAP " + mapOfUsername.keys())
                                            } else return
                                            name = !name.trim() ? mapOfUsername.get(from) as string : name
                                            setMessageList([...messageList, {text: text, from: name, type: type}])
                                            messageList = [...messageList, {text: text, from: name, type: type}]
                                        }
                                    }
                                }
                            )
                        },
                        error: function (error: string) {
                            console.error("ERROR", error)
                            setCurrentPageState(stateOfPage.CouldNotConnectToServer)
                        },
                        destroyed: function () {
                            window.location.reload()
                        }
                    }
                )
            }
        });
    }

    const {user} = useAuth()

    useEffect(() => {
        if (!isLoadingJanus) {
            isLoadingJanus = true
            InitJanus()
        }
    }, []);

    useEffect(() => {
        const handleLeavePage = async (event: any) => {
            event.preventDefault();
            const myRoom = localStorage.getItem("roomid") as string
            UserService.leaveRoom(myRoom).then(r => {
            })
            event.returnValue = 'Вы действительно хотите покинуть комнату?';
        };

        window.addEventListener('beforeunload', handleLeavePage);

        return () => {
            window.removeEventListener('beforeunload', handleLeavePage);
        };
    }, []);


    function sendData(text: string) {
        const myRoom = localStorage.getItem("roomid")
        let message = {
            textroom: "message",
            transaction: JanusJS.randomString(12),
            room: myRoom,
            text: text,
        };
        console.log(JSON.stringify(message))
        console.log("!!!SEND!!! " + textRoom)
        textRoom.data({
            data: JSON.stringify(message),
            error: function (reason: string) {
                toast.error(reason);
            },
            success: function () {
            }
        });
    }

    function setOperator(id: string) {
        const myRoom = localStorage.getItem("roomid")
        let message = {
            textroom: "set_operator",
            transaction: JanusJS.randomString(12),
            room: myRoom,
            username: id,
        };
        console.log(JSON.stringify(message))
        console.log("!!!SET OPERATOR!!! " + textRoom)
        textRoom.data({
            data: JSON.stringify(message),
            error: function (reason: string) {
                toast.error(reason);
            },
            success: function () {
            }
        });
    }

    const navigate = useNavigate();

    const onLeaveRoom = () => {
        const myRoom = localStorage.getItem("roomid") as string
        let message = {
            textroom: "leave",
            transaction: JanusJS.randomString(12),
            room: myRoom,
        };
        try {
            textRoom.data({
                data: JSON.stringify(message),
                error: function (reason: string) {
                    toast.error(reason);
                },
                success: function () {
                    UserService.leaveRoom(myRoom)
                }
            });
        } catch {
        } finally {
            navigate("/profile")
            localStorage.removeItem("roomid")
            localStorage.removeItem("userRoomToken")
            localStorage.removeItem("userOwnerId")
        }
    }

    function leaveFromRoom() {
        setUnpublish(true)
    }

    function registerUsername() {
        const userRoomToken = localStorage.getItem("userRoomToken")
        const userOwnerId = localStorage.getItem("userOwnerId")
        const myRoom = localStorage.getItem("roomid")
        if (!myRoom) {
            toast.error("Попробуй заново войти в комнату")
            leaveFromRoom()
        }
        let transaction = JanusJS.randomString(12);
        let register = {
            textroom: "join",
            transaction: transaction,
            room: myRoom,
            username: userOwnerId,
            display: user?.username,
            token: userRoomToken
        };
        console.log(`register: ${myRoom}`)
        myUsername = user?.username;
        textroom.data({
            data: JSON.stringify(register),
            error: function (reason: string) {
                setCurrentPageState(stateOfPage.RegisterUserError)
                toast.error(reason);
            },
            success: function () {
                setCurrentPageState(stateOfPage.Success)
            }
        });
    }

    const syncVideo = (isPlaying: boolean, currentTime: number) => {
        if (isPlaying != undefined && currentTime != undefined) {
            const myRoom = localStorage.getItem("roomid")
            let message = {
                textroom: "sync",
                room: myRoom,
                transaction: JanusJS.randomString(12),
                is_playing: isPlaying,
                time: currentTime
            };
            console.log(JSON.stringify(message))
            textRoom.data({
                data: JSON.stringify(message),
                error: function (reason: string) {
                    toast.error(reason);
                }
            });
        }
    }

    const selectMovieToPlay = (id: string) => {
        setMovieId(id)
        const myRoom = localStorage.getItem("roomid")
        setSyncedIsPlay(false)
        setSyncedTime(0.00000000001)
        let message = {
            textroom: "sync",
            room: myRoom,
            transaction: JanusJS.randomString(12),
            is_playing: false,
            time: 0.00000000001,
            current_movie: id
        };
        console.log(JSON.stringify(message))
        textRoom.data({
            data: JSON.stringify(message),
            error: function (reason: string) {
                toast.error(reason);
            }
        });
        setSyncedTime(0.1)
    }

    const onAddClicked = (id: number) => {
        if (movieIdList.includes(id) == undefined) {
        } else {
            const list = [...movieIdList, id]
            updateList(list)
        }
    }

    const onRemoveClicked = (id: number) => {
        const list = movieIdList.filter((it) => (it != id))
        updateList(list)
    }

    const playNext = () => {
        const list = movieIdList.filter((it) => ('' + it != movieId))
        setMovieId(list[0] != undefined ? '' + list[0] : '' + 0)
        updateList(list)
    }

    const updateList = (new_list: number[]) => {
        setMovieIdList(new_list)
        movieIdList = new_list
        const myRoom = localStorage.getItem("roomid")
        let message = {
            textroom: "sync",
            room: myRoom,
            transaction: JanusJS.randomString(12),
            playlist: new_list,
            current_movie: movieId
        };
        console.log(JSON.stringify(message))
        textRoom.data({
            data: JSON.stringify(message),
            error: function (reason: string) {
                toast.error(reason);
            }
        });
    }

    const ConnectingElement = <div>
        <h1>Connecting...</h1>
    </div>
    const RegisteringUser = <div>
        <h1>Registering user...</h1>
    </div>

    const Error = <div>
        Something went wrong
    </div>

    const NotExist = <div>
        Room not exist
    </div>

    const successPage =
        <div className="h-screen grid grid-cols-5">
            <div className="col-span-3 flex flex-col items-center justify-center relative">
                <div className="relative w-full">
                    <Player
                        movieId={movieId}
                        sync={syncVideo}
                        isOperator={myId == whoIsOperator}
                        syncedTime={syncedTime}
                        syncedPause={syncedIsPlay}
                        playNext={playNext}
                    />
                </div>
            </div>
            <div className="col-span-1 row-span-6 h-full bg-gray-100">
                <VideoRoom
                    roomId={localStorage.getItem("roomid") as string}
                    token={localStorage.getItem("userRoomToken") as string}
                    displayName={user?.username ? user.username : ""}
                    unpublish={unpublish}
                    onLeaveRoom={onLeaveRoom}
                />
            </div>
            <div className="col-span-1 row-span-6 h-full bg-gray-100">
                <Chat
                    messages={messageList}
                    chatMembers={mapOfUsername}
                    sendMessage={sendData}
                    leaveFromRoom={leaveFromRoom}
                    setOperator={setOperator}
                    isOwner={isOwner}
                    myId={myId as string}
                    operatorId={whoIsOperator}
                    movieList={movieList}
                    currentMovieId={movieId}
                    playlistMoviesIds={movieIdList}
                    playMovie={selectMovieToPlay}
                    onAddClicked={onAddClicked}
                    onRemoveClicked={onRemoveClicked}
                />
            </div>
        </div>


    return (
        <>
            {currentPageState == stateOfPage.Connecting ? (ConnectingElement) : (<></>)}
            {currentPageState == stateOfPage.RegisterUser ? (RegisteringUser) : (<></>)}
            {currentPageState == stateOfPage.Success ? (successPage) : (<></>)}
            {currentPageState == stateOfPage.WebRTCSupportError ? (Error) : (<></>)}
            {currentPageState == stateOfPage.CouldNotConnectToServer ? (Error) : (<></>)}
            {currentPageState == stateOfPage.AttachTextRoomError ? (Error) : (<></>)}
            {currentPageState == stateOfPage.RegisterUserError ? (Error) : (<></>)}
            {currentPageState == stateOfPage.RoomNotExist ? (NotExist) : (<></>)}
        </>
    )
}


