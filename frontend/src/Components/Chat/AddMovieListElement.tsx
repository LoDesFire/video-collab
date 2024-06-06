import React from "react";
import {MovieDto} from "../../Models/MovieDto";
import {MovieCardForRoom} from "./MovieCardForRoom";

type MoviesListProps =
    {
        movieList: MovieDto[],
        currentMovieId: number | undefined,
        playlistMoviesIds: number[]
        playMovie: (id: number) => void
        isOperator: boolean
    }

export const MoviesListElement = ({
                                      movieList,
                                      currentMovieId,
                                      playlistMoviesIds,
                                      playMovie,
                                      isOperator
                                  }: MoviesListProps) => {

    return (
        <>
            <h2 className="text-xl font-bold mb-4 pl-4 pt-4">Selected Movie</h2>
            <div className="flex flex-col h-full overflow-y-auto p-4">
                <div className="flex flex-col flex-grow">
                    {playlistMoviesIds.map((id, index) => (
                        <MovieCardForRoom
                            movie={movieList.filter(it => it.id == id)[0]}
                            addMovie={() => {
                            }}
                            removeMovie={() => {
                            }}
                            isAdded={true}
                            isCurrentMovie={currentMovieId == id}
                            playMovie={playMovie}
                            isOperator={isOperator}
                        />
                    ))}
                </div>
            </div>
        </>
    )
}