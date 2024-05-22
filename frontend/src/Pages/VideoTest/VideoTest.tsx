import React, {useEffect, useRef, useState} from "react";
import ReactPlayer from 'react-player/'
import {
    AiFillPlayCircle,
    AiFillPauseCircle,
    AiOutlineFullscreen,
    AiOutlineFullscreenExit,
    AiOutlinePauseCircle, AiOutlinePlayCircle
} from "react-icons/ai";
import {toast} from "react-toastify";
import fullscreen from "screenfull"
import {useParams} from "react-router-dom";
import Hls from "hls.js";
import {wait} from "@testing-library/user-event/dist/utils";

type VideoProps = {
    movieId: number,
    sync: (isPlaying: boolean, currentTime: number) => void,
    isOperator: boolean,
    syncedPause: boolean,
    syncedTime: number
}

export const VideoTestPage = ({movieId, sync, isOperator, syncedTime, syncedPause}: VideoProps) => {
    const refPlayer = useRef<ReactPlayer>(null)
    const containerRef = useRef(null);

    const tmpUrl = "/api/movie/watch/" + movieId + "/.m3u8"
    const [videoUrl, setVideoUrl] = useState(tmpUrl);

    const [state, setState] = useState({
        playing: false,
        volume: 0.5,
        played: 0,
        isFullScreen: false,
        listOfQualities: ["Auto"],
        qualitiesUrls: [tmpUrl],
        quality: 0,
        url: tmpUrl
    })

    useEffect(() => {
        let diff = syncedTime - state.played;
        if(diff < 0) diff *= -1
        if(diff > 0.05) {
            refPlayer?.current?.seekTo(Number(syncedTime), "fraction")
            setState({...state, played: Number(syncedTime)})
        }
    }, [syncedTime]);

    useEffect(() => {
        setState({...state, playing: syncedPause})
    }, [syncedPause]);

    const {
        playing,
        volume,
        played,
        isFullScreen,
        quality
    } = state

    useEffect(() => {
        if (Hls.isSupported()) {
            const hls = new Hls();
            hls.loadSource(tmpUrl);

            hls.on(Hls.Events.MANIFEST_PARSED, function (event, data) {

                const availableQualities = hls.levels
                let listOfQualities = availableQualities.map((l) => String(l.height))
                listOfQualities = ["Auto", ...listOfQualities]
                let qualitiesUrls = availableQualities.map((l) => String(l.url[0]))
                qualitiesUrls = [tmpUrl, ...qualitiesUrls]
                setState({...state, listOfQualities: listOfQualities, qualitiesUrls: qualitiesUrls})
            });
        }
    }, [movieId]);

    const handlePlay = () => {
        const cur = !state.playing
        setState({...state, playing: !state.playing})
        if(isOperator)
        sync(cur, state.played)
    }

    const handleVolume = (e: any) => {
        setState({...state, volume: Number(e.target.value)})
    }

    const handleProgress = (e: any) => {
        setState({...state, played: Number(e.played)})
        if(isOperator)
        sync(state.playing, Number(e.played))
    }

    const handleChangeProgress = (e: any) => {
        refPlayer?.current?.seekTo(Number(e.target.value), "fraction")
        setState({...state, played: Number(e.target.value)})
        if(isOperator)
        sync(state.playing, Number(e.target.value))
    }

    const handleFullScreen = () => {
        if (containerRef.current != null) {
            fullscreen.toggle(containerRef.current).then()
            setState({...state, isFullScreen: !isFullScreen})
        }
    }

    const handleChangeQuality = () => {
        const tmpQuality = (quality + 1) % state.listOfQualities.length
        setState({...state, quality: tmpQuality})
        setVideoUrl(state.qualitiesUrls[tmpQuality])
    }

    const playerButtonStyle = "fill-gray-100 h-4/5 bg-g"

    const playerFullScreenStyle = "fixed bottom-0 w-full p-4 bg-black"
    const playerNotFullScreenStyle = "fixed bottom-0 w-3/4 p-4 bg-white"

    const ControlsVideo =
        <>
            <div className={isFullScreen ? playerFullScreenStyle : playerNotFullScreenStyle}>
                <div className="flex items-center justify-between mt-2">
                    <button onClick={handlePlay} className="text-2xl bg-white rounded mr-2">
                        {playing ? <AiOutlinePauseCircle size={30}/> : <AiOutlinePlayCircle size={30}/>}
                    </button>
                    <input
                        type="range"
                        min="0"
                        max="1"
                        className="w-1/4"
                        step="0.000001"
                        value={volume}
                        onChange={handleVolume}
                    />
                    <input
                        type="range"
                        min="0"
                        max="1"
                        step="0.000001"
                        value={played}
                        onChange={handleChangeProgress}
                        className="w-full mx-2"
                    />
                    <button onClick={handleChangeQuality} className="text-darkBlue">
                        {state.listOfQualities[quality]}
                    </button>
                    <button onClick={handleFullScreen} className="text-2xl bg-white rounded ml-2">
                        {isFullScreen ? <AiOutlineFullscreenExit size={30}/> : <AiOutlineFullscreen size={30}/>}
                    </button>
                </div>
            </div>
        </>

    return (
        <>
            <div className="fixed left-0 w-3/4 ">
                <div ref={containerRef}>
                    <ReactPlayer
                        onReady={() => wait(110).then(() => {
                            refPlayer?.current?.seekTo(state.played, "fraction")
                        })}
                        url={videoUrl}
                        controls={false}
                        width="100%"
                        playing={playing}
                        volume={volume}
                        ref={refPlayer}
                        onProgress={handleProgress}
                        height="auto"
                    />
                    {isOperator && ControlsVideo}
                </div>
            </div>
        </>
    )
}