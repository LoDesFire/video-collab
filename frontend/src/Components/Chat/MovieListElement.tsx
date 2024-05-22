import React from "react";
import {MovieDto} from "../../Models/MovieDto";
import {MovieCard} from "../MovieCard/MovieCard";

type MoviesListProps =
    {
        movieList: MovieDto[]
    }

export const MoviesListElement = ({movieList}: MoviesListProps) => {
    return (
        <>
            <h2 className="text-xl font-bold mb-4 pl-4 pt-4">Movie</h2>
            <div className="flex flex-col h-full overflow-y-auto p-4">
                <div className="flex flex-col flex-grow">
                    {movieList.map((movie, index) => (
                        <MovieCard movie={movie}
                                   pinned={undefined}
                                   userPinMovie={() => {}}
                                   userUnpinMovie={() => {}}
                        />
                    ))}
                </div>
            </div>
        </>
    )
}