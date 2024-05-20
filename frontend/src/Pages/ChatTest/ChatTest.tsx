import adapter from 'webrtc-adapter'
import JanusJS from "janus-gateway"
import React, {useEffect, useState} from "react";
import {toast} from "react-toastify";
import {useAuth} from "../../Context/useAuth";
import {JanusServerWS, JanusTextRoomPlugin} from "../../Constants";
import {Chat} from "../../Components/Chat/Chat";
import {MessageModel} from "../../Components/Chat/MessageModel";
import {MessageTypesEnum} from "../../Components/Chat/MessageTypesEnum";
import {type} from "node:os";
import {leaveRoom} from "../../Services/UserService";
import {useNavigate} from "react-router-dom";

export const ChatTestPage = () => {

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

    let myUsername: string | undefined = "";

    const [dp, setDp] = useState(JanusJS.useDefaultDependencies({adapter: adapter}))
    const [currentPageState, setCurrentPageState] = useState(stateOfPage.Connecting)
    let janus: JanusJS;

    let [messageList, setMessageList] = useState<MessageModel[]>([])

    let [mapOfUsername, setMapOfUsername] = useState(new Map<string, string>())

    // @ts-ignore
    const [textRoom, setTextRoom] = useState<JanusJS.PluginHandle>();

    // @ts-ignore
    let textroom: JanusJS.PluginHandle;

    let isLoadingJanus = false;

    function InitJanus() {
        JanusJS.init({
            debug: true,
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
                                        toast(data as string)
                                        JanusJS.debug("We got data from the DataChannel!", data);
                                        //toast.success("ONDATA:" + data)
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
                                        } else if (what === "error") {
                                            if (json["error_code"] == 417) setCurrentPageState(stateOfPage.RoomNotExist)
                                            if (json["error_code"] == 419) setCurrentPageState(stateOfPage.RegisterUserError)
                                            if (json["error_code"] == 420) window.location.reload()
                                        } else {
                                            if (what === "message") {
                                                console.log("message")
                                                type = MessageTypesEnum.message
                                                from = json["from"]
                                                text = json["text"]
                                            } else
                                            if (what === "leave") {
                                                console.log("leave")
                                                type = MessageTypesEnum.leave
                                                from = json["username"]
                                                name = mapOfUsername.get(from) as string
                                                mapOfUsername.delete(from)
                                                setMapOfUsername(mapOfUsername)
                                            } else
                                            if (what === "join") {
                                                console.log("join")
                                                type = MessageTypesEnum.join
                                                from = json["username"]
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
            leaveRoom(myRoom).then(r => {
            })
            event.returnValue = 'Вы действительно хотите покинуть комнату?'; // Работает только с beforeunload
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

    const [input, setInput] = useState<string>('');

    function sendRandomData() {
        textRoom.data({
            data: input,
            success: function () {
                toast.success("SUCCESS")
            },
            error: function () {
                toast.error("ERROR")
            }
        })
    }

    const navigate = useNavigate();

    function leaveFromRoom() {
        const myRoom = localStorage.getItem("roomid") as string
        let message = {
            textroom: "leave",
            transaction: JanusJS.randomString(12),
            room: myRoom,
        };
        textRoom.data({
            data: JSON.stringify(message),
            error: function (reason: string) {
                toast.error(reason);
            },
            success: function () {
                leaveRoom(myRoom).then(r => {
                })
                navigate("/profile")
            }
        });
    }

    function registerUsername() {
        const userRoomToken = localStorage.getItem("userRoomToken")
        const userOwnerId = localStorage.getItem("userOwnerId")
        const myRoom = localStorage.getItem("roomid")
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
                toast.success("Register")
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


    return (
        <>
            <div>
                <textarea
                    value={input}
                    onChange={(e) => setInput(e.target.value)}
                />
                <button onClick={sendRandomData}>SEND IT!!!</button>
            </div>
            {currentPageState == stateOfPage.Connecting ? (ConnectingElement) : (<></>)}
            {currentPageState == stateOfPage.RegisterUser ? (RegisteringUser) : (<></>)}
            {currentPageState == stateOfPage.Success ? (
                <div className="flex justify-center items-center h-screen bg-gray-100">
                    <div className="w-full max-w-md h-full bg-white shadow-lg border rounded">
                        <Chat
                            messages={messageList}
                            chatMembers={mapOfUsername}
                            sendMessage={sendData}
                            leaveFromRoom={leaveFromRoom}
                        />
                    </div>
                </div>
            ) : (<></>)}
            {currentPageState == stateOfPage.WebRTCSupportError ? (Error) : (<></>)}
            {currentPageState == stateOfPage.CouldNotConnectToServer ? (Error) : (<></>)}
            {currentPageState == stateOfPage.AttachTextRoomError ? (Error) : (<></>)}
            {currentPageState == stateOfPage.RegisterUserError ? (Error) : (<></>)}
            {currentPageState == stateOfPage.RoomNotExist ? (NotExist) : (<></>)}
        </>
    )
}


