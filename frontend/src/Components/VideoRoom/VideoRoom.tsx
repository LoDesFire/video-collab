import {createVideoRoomClient} from "janus-simple-videoroom-client";
import React, {useEffect, useState} from 'react';
import {JanusServerWS} from "../../Constants";
import {ReactComponent as VideoMuted} from './icons/video-muted.svg';
import {ReactComponent as VideoNotMuted} from './icons/video-not-muted.svg';
import {ReactComponent as MicroMuted} from './icons/microphone-muted.svg';
import {ReactComponent as MicroNotMuted} from './icons/microphone-not-muted.svg';
import {toast} from "react-toastify";

const makeDisplay = (displayName: string, isMe: boolean) => {
    console.log("Make Display, Is Me? : " + isMe)

    const stream = new MediaStream();
    const displayElement = document.createElement('div');
    displayElement.className = 'display relative inline-block m-4';

    const nameElement = document.createElement('div');
    nameElement.className = 'name absolute bg-black text-white';
    nameElement.textContent = displayName;

    const videoElement = document.createElement('video');
    videoElement.autoplay = true;
    videoElement.srcObject = stream;
    videoElement.muted = isMe;

    displayElement.appendChild(nameElement);
    displayElement.appendChild(videoElement);
    if (isMe) {
        const div = document.getElementById('display')
        while (div?.firstChild) {
            div.removeChild(div.firstChild);
        }
        console.log(`My video removed`)
        div?.appendChild(displayElement)
        console.log(`My video added`)
    } else {
        const div = document.getElementById('displays')
        div?.childNodes.forEach((child) => {
            if (child?.firstChild?.textContent == displayName) {
                console.log(`${displayName} removed`)
                div.removeChild(child);
            }
        })
        document.getElementById('displays')?.appendChild(displayElement)
        console.log(`${displayName} added`)

    }

    return {
        stream: stream,
        remove: () => displayElement.remove()
    };
};

interface TestVideoRoomProps {
    roomId: string,
    token: string,
    displayName: string,
    unpublish: boolean,
    onLeaveRoom: () => void,
}

export const VideoRoom = ({roomId, displayName, token, unpublish, onLeaveRoom}: TestVideoRoomProps) => {
    const [server, setServer] = useState(JanusServerWS);
    const [clientReady, setClientReady] = useState<any>(null);
    let [room, setRoom] = useState<any>(null);
    let [pub, setPub] = useState<any>(null);

    const [isMuted, setIsMuted] = useState(false);
    const [isCameraOff, setIsCameraOff] = useState(false);

    useEffect(() => {
        const client = createVideoRoomClient({debug: true});
        setClientReady(client);
    }, []);

    useEffect(() => {
        if (clientReady) {
            connect(server, roomId, displayName, token);
        }
    }, [clientReady]);

    useEffect(() => {
        console.log("Unpublish" + unpublish)
        if (unpublish) {
            try {
                pub.unpublish()
            } catch (e) {
            }
            onLeaveRoom()
        }
    }, [unpublish]);

    useEffect(() => {
        if (pub) {
            console.log(`Something change: Camera off: ${isCameraOff}, Muted? : ${isMuted}`)
            pub.muteMyVideo(isCameraOff)
            pub.muteMyAudio(isMuted)
        }
    }, [isCameraOff, isMuted]);

    const connect = async (server: string, roomId: string, displayName: string, token: string) => {
        if (!clientReady) return;

        const client = await clientReady;
        const session = await client.createSession(server);
        const t_room = await session.joinRoom(roomId, token);
        room = t_room

        setRoom(t_room)

        await my_connect()

        const subs: any = {};
        room.onPublisherAdded((publishers: any[]) => publishers.forEach((publisher: any) => subscribe(publisher, room, subs)));
        room.onPublisherRemoved((publisherId: any) => unsubscribe(publisherId, subs));
    };

    const my_connect = async () => {
        console.log(`Is muted: ${isMuted}`)
        let tracks: any[] = [
            {type: "audio", capture: !isMuted},
            {type: "video", capture: !isCameraOff}
        ]
        try {
            const tpub = await room.publish({
                publishOptions: {
                    display: displayName,
                },
                mediaOptions: {
                    tracks: tracks
                }
            })
            console.log(tpub)
            setPub(tpub)
            pub = tpub
            const myVideo = makeDisplay(displayName, true);
            tpub.onTrackAdded((track: MediaStreamTrack) => myVideo.stream.addTrack(track));
            tpub.onTrackRemoved((track: MediaStreamTrack) => myVideo.stream.removeTrack(track));
        } catch {
            toast.error("Что-то пошло не так... Попробуй перезагрузить страницу")
        }
    }

    const subscribe = async (publisher: any, room: any, subs: any) => {
        console.log(`New subscriber: ${publisher.id} ${publisher.display}`)
        const sub = subs[publisher.id] = await room.subscribe([{feed: publisher.id}]);
        sub.video = makeDisplay(publisher.display, false);
        sub.onTrackAdded((track: MediaStreamTrack) => sub.video.stream.addTrack(track));
        sub.onTrackRemoved((track: MediaStreamTrack) => sub.video.stream.removeTrack(track));
    };

    const unsubscribe = async (publisherId: string, subs: any) => {
        console.log(`Remove subscriber: ${publisherId}`)
        await subs[publisherId].unsubscribe();
        subs[publisherId].video.remove();
    };

    const leave = () => {
        pub.unpublish()
        console.log(`Unpublished`)
    }

    const toggleMute = () => {
        console.log(pub)
        setIsMuted(!isMuted);
    };


    const toggleCamera = () => {
        setIsCameraOff(!isCameraOff);
    };

    return (
        <div>
            <div id="display" className="flex flex-col p-4 space-y-4"></div>
            <div>
                <button className="pl-10" onClick={toggleMute}>
                    {isMuted ? <MicroMuted/> : <MicroNotMuted/>}
                </button>
                <button className="pl-5" onClick={toggleCamera}>
                    {isCameraOff ? <VideoMuted/> : <VideoNotMuted/>}
                </button>
            </div>
            <div id="displays" className="flex flex-col p-4 space-y-4"></div>
        </div>
    );
};
