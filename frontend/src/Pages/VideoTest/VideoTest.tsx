import React, {useEffect, useRef, useState} from "react";
import ReactPlayer from 'react-player/'
import {AiFillPlayCircle, AiFillPauseCircle, AiOutlineFullscreen, AiOutlineFullscreenExit} from "react-icons/ai";
import {toast} from "react-toastify";
import fullscreen from "screenfull"
import {useParams} from "react-router-dom";
import Hls from "hls.js";
import {wait} from "@testing-library/user-event/dist/utils";
import {string} from "yup";


export const VideoTestPage = () => {
    const refPlayer = useRef<ReactPlayer>(null)
    const containerRef = useRef(null);

    let {id} = useParams();

    const tmpUrl = "/api/movie/watch/" + id + "/.m3u8"
    const [videoUrl, setVideoUrl] = useState(tmpUrl);

    const [state, setState] = useState({
        visible: true,
        playing: false,
        volume: 0.5,
        played: 0,
        isFullScreen: false,
        listOfQualities: ["Auto"],
        qualitiesUrls: [tmpUrl],
        quality: 0,
        url: tmpUrl
    })

    const {
        playing,
        volume,
        played,
        isFullScreen,
        quality
    } = state

    window.onload = (): void => {
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
    }

    const handlePlay = () => {
        setState({...state, playing: !state.playing, visible: state.playing})
    }

    const OnOverEnter = () => {
        setState({...state, visible: true})
    }

    const OnOverExit = () => {
        if (state.playing)
            setState({...state, visible: false})
    }

    const handleVolume = (e: any) => {
        setState({...state, volume: Number(e.target.value)})
    }

    const handleProgress = (e: any) => {
        setState({...state, played: Number(e.played)})
    }

    const handleChangeProgress = (e: any) => {
        refPlayer?.current?.seekTo(Number(e.target.value), "fraction")
        setState({...state, played: Number(e.target.value)})
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
    const ControlsVideo =
        <>
            <div className="absolute left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2 w-15 h-15 opacity-70"
                 onClick={handlePlay}>
                {!playing ?
                    <AiFillPlayCircle size={40} className={playerButtonStyle} onClick={handlePlay}/> :
                    <AiFillPauseCircle size={40} className={playerButtonStyle} onClick={handlePlay}/>
                }
            </div>
            <div className="absolute bottom-0 left-0 right-0 h-11 justify-center">
                <div className="flex space-x-4 h-11 justify-center bg-gray-700 opacity-90">
                    <div onClick={handlePlay} className="top-1/2">
                        {!playing ? <AiFillPlayCircle size={40} className={playerButtonStyle}/> :
                            <AiFillPauseCircle size={40} className={playerButtonStyle}/>}
                    </div>
                    <input
                        type="range"
                        min="0"
                        max="1"
                        className="w-1/4"
                        step="0.0001"
                        value={volume}
                        onChange={handleVolume}
                    />
                    <input
                        type="range"
                        className="w-2/3"
                        min="0"
                        max="1"
                        step="0.0000001"
                        value={played}
                        onChange={handleChangeProgress}
                    />
                    <div onClick={handleChangeQuality}>
                        {state.listOfQualities[quality]}
                    </div>
                    {!isFullScreen ?
                        <AiOutlineFullscreen size={40} onClick={handleFullScreen} className={playerButtonStyle}/> :
                        <AiOutlineFullscreenExit size={40} onClick={handleFullScreen} className={playerButtonStyle}/>
                    }
                </div>
            </div>
        </>
    return (
        <div ref={containerRef} className="absolute w-3/4 left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2"
             onMouseEnter={OnOverEnter} onMouseLeave={OnOverExit}>
            <ReactPlayer
                onReady={() => wait(100).then(() => {
                    refPlayer?.current?.seekTo(state.played, "fraction")
                })
                }
                url={videoUrl}
                controls={false}
                width="100%"
                playing={playing}
                volume={volume}
                ref={refPlayer}
                onProgress={handleProgress}
                height="auto"
                loop={true}
                onClick={handlePlay}
            />
            {state.visible ? (ControlsVideo) : (<></>)}
        </div>
    )
}

