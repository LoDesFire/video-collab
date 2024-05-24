import React from "react";
import {MovieDto} from "../../Models/MovieDto";
import {MovieCard} from "../MovieCard/MovieCard";
import {MovieCardForRoom} from "./MovieCardForRoom";

type MoviesListProps =
    {
        movieList: MovieDto[],
        currentMovieId: number | undefined,
        playlistMoviesIds: number[],
        onAddClicked: (id: number) => void
        onRemoveClicked: (id: number) => void
        isOperator: boolean
    }

export const AddMoviesListElement = ({
                                         movieList,
                                         currentMovieId,
                                         playlistMoviesIds,
                                         onAddClicked,
                                         onRemoveClicked,
                                         isOperator
                                     }: MoviesListProps) => {

    return (
        <>
            <h2 className="text-xl font-bold mb-4 pl-4 pt-4">All Movies</h2>
            <div className="flex flex-col h-full overflow-y-auto p-4">
                <div className="flex flex-col flex-grow">
                    {movieList.map((movie, index) => (
                        <MovieCardForRoom
                            movie={movie}
                            addMovie={onAddClicked}
                            removeMovie={onRemoveClicked}
                            isAdded={playlistMoviesIds.includes(movie.id)}
                            isCurrentMovie={currentMovieId == movie.id}
                            playMovie={undefined}
                            isOperator={isOperator}
                        />
                    ))}
                </div>
            </div>
        </>
    )
}