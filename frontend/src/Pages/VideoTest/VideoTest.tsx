import React, {useRef, useState} from "react";
import ReactPlayer from 'react-player/'
import {AiFillPlayCircle, AiFillPauseCircle} from "react-icons/ai";
import {toast} from "react-toastify";
import fullscreen from "screenfull"
import {useParams} from "react-router-dom";


export const VideoTestPage = () => {
    const playerRef = useRef<ReactPlayer>();
    const containerRef = useRef(null);

    let {id} = useParams();

    const [state, setState] = useState({
        playing: false,
        volume: 0.5,
        played: 0,
        isFullScreen: false
    })

    const {
        playing,
        volume,
        played,
        isFullScreen
    } = state

    const refPlayer = useRef<ReactPlayer>(null)

    const handlePlay = () => {
        setState({...state, playing: !state.playing})
    }

    const handleVolume = (e: any) => {
        setState({...state, volume: Number(e.target.value)})
    }

    const handleProgress = (e: any) => {
        if(playerRef.current != null)
            console.log(playerRef.current?.getInternalPlayer('hls').levels)
        setState({...state, played: Number(e.played)})
    }

    const handleChangeProgress = (e: any) => {
        if (refPlayer.current != null) {
            console.log(playerRef.current?.getInternalPlayer('dash'))
            refPlayer.current.seekTo(Number(e.target.value), "fraction")
        }
        setState({...state, played: Number(e.target.value)})
    }

    const handleFullScreen = () => {
        if (containerRef.current != null)
            fullscreen.toggle(containerRef.current).then()
    }

    const playerButtonStyle = "fill-gray-100"
    const ControlsVideo =
        <>
            <div className="absolute left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2 w-10 h-10 opacity-90"
                 onClick={handlePlay}>
                {!playing ? <AiFillPlayCircle size={40} className={playerButtonStyle}/> :
                    <AiFillPauseCircle size={40} className={playerButtonStyle}/>}
            </div>
            <div className="absolute bottom-0 left-0 right-0 h-16 justify-center">
                <input
                    type="range"
                    min="0"
                    max="1"
                    step="0.0001"
                    value={volume}
                    onChange={handleVolume}
                />
                <input
                    type="range"
                    className="w-1/2"
                    min="0"
                    max="1"
                    step="0.0000001"
                    value={played}
                    onChange={handleChangeProgress}
                />
                <button className="bg-white" onClick={handleFullScreen}>
                    Full
                </button>
            </div>
        </>

    const url = "/api/movie/watch/" + id + "/.m3u8"
    return (

        <div ref={containerRef} className="absolute w-1/2 left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2">
            <ReactPlayer
                url={url}
                controls={false}
                width="100%"
                playing={playing}
                volume={volume}
                ref={refPlayer}
                onProgress={handleProgress}
                height="auto"
                loop={true}
            />
            {ControlsVideo}
        </div>

    )
}