import {MovieDto} from "../../Models/MovieDto";
import React, {useState} from "react";
import {Link} from "react-router-dom";
import {AiFillStar, AiTwotoneStar} from 'react-icons/ai';

interface Props {
    movie: MovieDto;
    pinned: boolean | undefined;
    userPinMovie: (id: number) => void;
    userUnpinMovie: (id: number) => void;
}

export const MovieCard = ({movie, pinned, userPinMovie, userUnpinMovie}: Props) => {

    const [isPinned, setPinned] = useState<boolean | undefined>(pinned)

    return (
        <div className="bg-white w shadow rounded-lg overflow-hidden">
            <Link to={'/movie/' + movie.id}>
                <img src={movie.imageUrl} className="object-cover h-52 w-full hover:scale-105"
                     alt=""/>
            </Link>
            <div className="p-6">
                <div className="flex w-full justify-between">
                    <Link to={'/movie/' + movie.id}>
                        <h2 className="mt-1 font-bold text-lg pb-2 ">
                            {movie.name}
                        </h2>
                    </Link>
                    {isPinned != undefined && (
                        isPinned ? (
                            <AiFillStar size={30}
                                        className="fill-green-700 hover:fill-green-600"
                                        onClick={() => {
                                            userUnpinMovie(movie.id)
                                            setPinned(false)
                                        }}/>
                        ) : (
                            <AiTwotoneStar size={30}
                                           className="fill-green-700 hover:fill-green-600"
                                           onClick={() => {
                                               userPinMovie(movie.id)
                                               setPinned(true)
                                           }}/>
                        ))
                    }
                </div>
                <h3 className="mt-1 text-sm pb-1">
                    {movie.description}
                </h3>
            </div>
        </div>
    )
}