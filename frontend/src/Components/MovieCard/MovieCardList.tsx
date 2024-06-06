import {MovieDto} from "../../Models/MovieDto";
import {MovieCard} from "./MovieCard";

interface Props {
    savedMoviesResult: MovieDto[];
    listOfPinned: Map<number, boolean>;
    userPinMovie: (id: number) => void;
    userUnpinMovie: (id: number) => void;
}

export const MovieList = ({savedMoviesResult, listOfPinned, userUnpinMovie, userPinMovie}: Props) => {
    return (
        <div className="container mx-auto my-10">
            <div className="grid md:grid-cols-2 sm:grid-cols-1 lg:grid-cols-4 gap-4">
                {
                    savedMoviesResult.length > 0 ? (
                        savedMoviesResult.map((movie) => {

                            let pinned = listOfPinned?.get(movie.id) ? listOfPinned?.get(movie.id) : false
                            if (pinned == undefined) pinned = false
                            return (
                                <MovieCard
                                    movie={movie}
                                    pinned={pinned}
                                    userPinMovie={(id: number) => userPinMovie(id)}
                                    userUnpinMovie={(id: number) => userUnpinMovie(id)}
                                />
                            )
                        })
                    ) : (
                        <p className="mb-3 mt-3 text-xl font-semibold text-center md:text-xl">
                            Нет сохранненых
                        </p>
                    )
                }
            </div>
        </div>
    )
}