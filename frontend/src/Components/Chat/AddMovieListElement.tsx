import React, {useState} from "react";
import {MovieDto} from "../../Models/MovieDto";
import {MovieCardForRoom} from "./MovieCardForRoom";
import {toast} from "react-toastify";
import {AiOutlineSend} from "react-icons/ai";

type MoviesListProps =
    {
        movieList: MovieDto[],
        currentMovieId: string | undefined,
        playlistMoviesIds: number[]
        playMovie: (id: string) => void
        isOperator: boolean
    }

export const MoviesListElement = ({
                                      movieList,
                                      currentMovieId,
                                      playlistMoviesIds,
                                      playMovie,
                                      isOperator
                                  }: MoviesListProps) => {

    const [input, setInput] = useState("")

    const handlePressEnter = (e: React.KeyboardEvent<HTMLInputElement>) => {
        if (e.key == "Enter")
            handleSend()
    }

    const handleSend = () => {
        if (!input.includes(String("https://www.youtube.com/"), 0)) {
            toast.info("Вставьте ссылку на https://www.youtube.com/")
        } else {
            playMovie(input)
        }
    }


    return (
        <>
            <h2 className="text-xl font-bold mb-4 pl-4 pt-4">Selected Movie</h2>
            <div className="flex flex-col h-full overflow-y-auto p-4">
                {isOperator && (
                    <>
                        <label className="pl-5 font-bold text-lg">Play Youtube</label>
                        <div className="p-4 bg-gray-100 border-t flex ">
                            <input
                                type="text"
                                className="flex-grow p-2 border rounded-l"
                                value={input}
                                onChange={(e) => setInput(e.target.value)}
                                onKeyDown={handlePressEnter}
                            />
                            <button
                                onClick={handleSend}
                                className="p-2 bg-green-700 text-white rounded-r hover:bg-green-800 flex items-center justify-center"
                            >
                                Watch
                            </button>
                        </div>
                    </>
                )
                }
                <div className="flex flex-col flex-grow">
                    {playlistMoviesIds.map((id, index) => (
                        <MovieCardForRoom
                            movie={movieList.filter(it => it.id == id)[0]}
                            addMovie={() => {
                            }}
                            removeMovie={() => {
                            }}
                            isAdded={true}
                            isCurrentMovie={currentMovieId == '' + id}
                            playMovie={playMovie}
                            isOperator={isOperator}
                        />
                    ))}
                </div>
            </div>
        </>
    )
}