import React, {useRef, useState} from "react";
import ReactPlayer from 'react-player/'
import {AiFillPlayCircle, AiFillPauseCircle} from "react-icons/ai";
import { toast } from "react-toastify";


export const VideoTestPage = () => {
    const playerRef = useRef<ReactPlayer>();

    const [state, setState] = useState({
        playing: false,
        volume: 0.5,
        played: 0
    })

    const {
        playing,
        volume,
        played
    } = state

    const refPlayer = useRef(null)

    const handlePlay = () => {
        setState({...state, playing: !state.playing})
    }

    const handleVolume = (e: any) => {
        setState({...state, volume: Number(e.target.value)})
    }

    const handleProgress = (e: any) => {
        console.log(e.played)
        setState({...state, played: Number(e.played)})
    }

    const handleChangeProgress = (e: any) => {
        setState({...state, played: Number(e.target.value)})
    }

    const ControlsVideo = () => {
        return (
            <>
                <div className="absolute left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2 bg-yellow-200 w-10 h-10"
                     onClick={handlePlay}>
                    {!playing ? <AiFillPlayCircle/> : <AiFillPauseCircle/>}
                </div>
                <div className="absolute bottom-0 left-0 right-0 h-16 justify-center">
                    <input
                        type="range"
                        min="0"
                        max="1"
                        step="0.1"
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
                </div>
            </>
        )
    }

    return (
        <div className="relative w-1/2">
            <ReactPlayer
                url="api/movie/1/watch"
                controls={true}
                width="100%"
                /*playing={playing}
                volume={volume}
                ref={refPlayer}
                onProgress={handleProgress}*/
                height="auto"
            />
            {/*<ControlsVideo/>*/}
        </div>
    )
}