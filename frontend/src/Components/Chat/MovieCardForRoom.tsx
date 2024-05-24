import {MovieDto} from "../../Models/MovieDto";
import React, {useEffect, useState} from "react";
import {Link, useNavigate} from "react-router-dom";
import {AiFillCamera, AiFillStar, AiFillVideoCamera, AiOutlineVideoCameraAdd, AiTwotoneStar} from 'react-icons/ai';

interface Props {
    movie: MovieDto;
    isAdded: boolean;
    addMovie: (id: number) => void;
    removeMovie: (id: number) => void;
    playMovie: ((id: number) => void) | undefined
    isCurrentMovie: boolean
    isOperator: boolean
}

export const MovieCardForRoom = ({
                                     movie,
                                     isAdded,
                                     addMovie,
                                     removeMovie,
                                     playMovie,
                                     isCurrentMovie,
                                     isOperator
                                 }: Props) => {

    const handleWatch = () => {
        if (playMovie)
            playMovie(movie.id)
    }

    const curr = "bg-gray-100 w shadow rounded-lg overflow-hidden mb-4"
    const notCurr = "bg-white w shadow rounded-lg overflow-hidden mb-4"

    return (
        <div className={isCurrentMovie ? curr : notCurr}>
            <img src={movie.imageUrl} className="object-cover h-52 w-full hover:scale-105"
                 alt=""/>
            <div className="p-6">
                <h2 className="mt-1 font-bold text-lg pb-2 ">
                    {movie.name}
                </h2>
                <div className="flex w-full justify-between">
                    {(
                        !isAdded ? (
                            <AiOutlineVideoCameraAdd size={30}
                                                     className="fill-green-700 hover:fill-green-600"
                                                     onClick={() => {
                                                         addMovie(movie.id)
                                                     }}/>
                        ) : (
                            <AiFillCamera size={30}
                                          className="fill-green-700 hover:fill-green-600"
                                          onClick={() => {
                                              removeMovie(movie.id)
                                          }}/>
                        ))
                    }
                </div>
                {isOperator && playMovie && !isCurrentMovie && <button onClick={handleWatch}>Watch</button>}

            </div>
        </div>
    )
}