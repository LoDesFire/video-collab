import React, {useEffect, useRef, useState} from "react";
import ReactPlayer from 'react-player/'
import {
    AiOutlineFullscreen,
    AiOutlineFullscreenExit,
    AiOutlinePauseCircle, AiOutlinePlayCircle
} from "react-icons/ai";
import fullscreen from "screenfull"
import Hls from "hls.js";
import {wait} from "@testing-library/user-event/dist/utils";

type VideoProps = {
    movieId: number,
    sync: (isPlaying: boolean, currentTime: number) => void,
    isOperator: boolean,
    syncedPause: boolean,
    syncedTime: number,
    playNext: () => void
}

export const VideoTestPage = ({movieId, sync, isOperator, syncedTime, syncedPause, playNext}: VideoProps) => {
    const refPlayer = useRef<ReactPlayer>(null)
    const containerRef = useRef(null);

    let tmpUrl = "/api/movie/watch/" + movieId + "/.m3u8"
    const [duration, setDuration] = useState<number>(0)
    const [videoUrl, setVideoUrl] = useState(tmpUrl);

    const [state, setState] = useState({
        playing: false,
        volume: 0.5,
        playedSec: 0,
        isFullScreen: false,
        listOfQualities: ["Auto"],
        qualitiesUrls: [tmpUrl],
        quality: 0,
        url: tmpUrl
    })

    useEffect(() => {
        let diff = syncedTime - state.playedSec;
        if (diff < -1 || diff > 1) {
            refPlayer?.current?.seekTo(Number(syncedTime), "seconds")
            setState({...state, playedSec: Number(syncedTime)})
        }
    }, [syncedTime]);

    useEffect(() => {
        setState({...state, playing: syncedPause})
    }, [syncedPause]);

    const {
        playing,
        volume,
        isFullScreen,
        quality,
        playedSec
    } = state

    useEffect(() => {
        if (movieId == 0) return
        let tempUrl = "/api/movie/watch/" + movieId + "/.m3u8"
        setVideoUrl(tempUrl)
        refPlayer?.current?.seekTo(state.playedSec, "seconds")
        if (Hls.isSupported()) {
            const hls = new Hls();
            hls.loadSource(tmpUrl);
            hls.on(Hls.Events.MANIFEST_PARSED, function (event, data) {
                const availableQualities = hls.levels
                setDuration(refPlayer?.current?.getDuration() as number)
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
        if (isOperator)
            sync(cur, Number(state.playedSec) + 0.0001)
    }

    const handleNothing = () => {
    }

    const handleVolume = (e: any) => {
        setState({...state, volume: Number(e.target.value)})
    }

    const handleProgress = (e: any) => {
        console.log(e)
        setState({...state, playedSec: Number(e.playedSeconds)})
        if (isOperator)
            sync(state.playing, Number(e.playedSeconds) + 0.0001)
    }

    const handleChangeProgress = (e: any) => {
        refPlayer?.current?.seekTo(Number(e.target.value), "seconds")
        setState({...state, playedSec: Number(e.target.value)})
        if (isOperator)
            sync(state.playing, Number(e.target.value) + 0.0001)
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
    const playerNotFullScreenStyle = "fixed bottom-0 w-3/5 p-4 bg-white"

    const ControlsVideo =
        <>
            <div className={isFullScreen ? playerFullScreenStyle : playerNotFullScreenStyle}>
                <div className="flex items-center justify-between mt-2">
                    <button onClick={isOperator ? handlePlay : handleNothing}
                            className="text-2xl bg-white rounded mr-2">
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
                        max={duration}
                        step="1"
                        value={playedSec}
                        onChange={isOperator ? handleChangeProgress : handleNothing}
                        className="w-full mx-2"
                    />
                    <h1>{Math.floor(state.playedSec) + "/" + Math.floor(duration) + "c"}</h1>
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
            {movieId != 0 && (<div className="fixed left-0 w-3/5 ">
                <div ref={containerRef}>
                    <ReactPlayer
                        onReady={() => wait(110).then(() => {
                            if (isOperator)
                                refPlayer?.current?.seekTo(state.playedSec, "seconds")
                        })}
                        onDuration={() => {
                            setDuration(refPlayer?.current?.getDuration() ? refPlayer?.current.getDuration() : 0)
                        }}
                        /*onEnded={playNext}*/
                        url={videoUrl}
                        controls={false}
                        width="100%"
                        playing={playing}
                        volume={volume}
                        ref={refPlayer}
                        onProgress={handleProgress}
                        height="auto"
                    />
                    {ControlsVideo}
                </div>
            </div>)
            }
            {
                movieId == 0 && (
                    <div>
                        Видео не выбрано.
                    </div>
                )
            }
        </>
    )
}