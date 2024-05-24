import {createVideoRoomClient} from "janus-simple-videoroom-client"
import React, {useState, useEffect, useRef} from 'react';
import {toast} from "react-toastify";
import {JanusServerWS} from "../../Constants";

interface DisplayProps {
    displayName: string;
}

const makeDisplay = (displayName: string, isMe: boolean) => {
    const stream = new MediaStream();
    const displayElement = document.createElement('div');
    displayElement.className = 'display relative inline-block m-4';

    const nameElement = document.createElement('div');
    nameElement.className = 'name absolute bg-black text-white';
    nameElement.textContent = displayName;

    const videoElement = document.createElement('video');
    videoElement.autoplay = true;
    videoElement.srcObject = stream;
    videoElement.muted = isMe

    displayElement.appendChild(nameElement);
    displayElement.appendChild(videoElement);
    document.getElementById('displays')?.appendChild(displayElement);

    return {
        stream: stream,
        remove: () => displayElement.remove()
    };
};

interface TestVideoRoomProps {
    roomId: string,
    token: string,
    displayName: string
}

export const TestVideoRoom = ({roomId, displayName, token}: TestVideoRoomProps) => {
    const [server, setServer] = useState(JanusServerWS);
    const [clientReady, setClientReady] = useState<any>(null);
    const [connection, setConnection] = useState<any>(null);

    let [listOfJoined, setListOfJoined] = useState<any[]>([])
    let [mapOfId, setMapOfId] = useState(new Map<string, string>)

    useEffect(() => {
        const client = createVideoRoomClient({debug: true});
        setClientReady(client);
    }, []);

    useEffect(() => {
        connect(server, roomId, displayName, token)
    }, [clientReady]);

    const connect = async (server: string, roomId: string, displayName: string, token: string) => {
        if (!clientReady) return;

        const client = await clientReady;
        const session = await client.createSession(server);
        const room = await session.joinRoom(roomId, token);

        if (!listOfJoined.includes(displayName)) {
            const pub = await room.publish({
                publishOptions: {
                    display: displayName,
                },
                mediaOptions: {
                    tracks: [
                        {type: "audio", capture: true},
                        {type: "video", capture: "lowres"}
                    ]
                }
            });

            const myVideo = makeDisplay(displayName, true);
            let tmp = [...listOfJoined, displayName]
            setListOfJoined(tmp)
            listOfJoined = tmp
            pub.onTrackAdded((track: MediaStreamTrack) => myVideo.stream.addTrack(track));
            pub.onTrackRemoved((track: MediaStreamTrack) => myVideo.stream.removeTrack(track));

            const subs: any = {};
            room.onPublisherAdded((publishers: any[]) => publishers.forEach((publisher: any) => subscribe(publisher, room, subs)));
            room.onPublisherRemoved((publisherId: any) => unsubscribe(publisherId, subs));

            setConnection({session, room, publisher: pub, subscribers: subs});
        }
    };

    const subscribe = async (publisher: any, room: any, subs: any) => {
        console.log(listOfJoined + "|||||")
        if (!listOfJoined.includes(publisher.display)) {
            let tmpMap = mapOfId
            tmpMap.set(publisher.id, publisher.display)
            mapOfId = tmpMap
            setMapOfId(tmpMap)
            const sub = subs[publisher.id] = await room.subscribe([{feed: publisher.id}]);
            sub.video = makeDisplay(publisher.display, false);
            sub.onTrackAdded((track: MediaStreamTrack) => sub.video.stream.addTrack(track));
            sub.onTrackRemoved((track: MediaStreamTrack) => sub.video.stream.removeTrack(track));
            let tmp = [...listOfJoined, publisher.display]
            setListOfJoined(tmp)
            listOfJoined = tmp
        }
    };

    const unsubscribe = async (publisherId: string, subs: any) => {
        await subs[publisherId].unsubscribe();
        subs[publisherId].video.remove();
        setListOfJoined(listOfJoined.filter((it) => (mapOfId.get(publisherId) != it)))
        listOfJoined = listOfJoined.filter((it) => (mapOfId.get(publisherId) != it))
    };

    useEffect(() => {
        connect(server, roomId, displayName, token)
            .then(() => document.getElementById('main-form')?.classList.add('hidden'))
            .catch(console.error);
    }, []);

    return (
        <div>
            <div id="displays" className="flex flex-col p-4 space-y-4"></div>
        </div>
    );
};
