import axios from "axios";
import {handleError} from "../Helper/ErrorHandler";
import {CreateMovieDto, MovieDto, MovieItemDto} from "../Models/MovieDto";

export class MovieService {

    static async movieAddNew(name: string, description: string, trailerLink: string, posterLink: string,) {
        try {
            return await axios.post<CreateMovieDto>("movie", {
                name: name,
                description: description,
                trailerLink: trailerLink,
                posterLink: posterLink,
            })
        } catch (error) {
            handleError(error)
        }
    }

    static async getAllMovie() {
        try {
            return axios.get<MovieDto[]>("movie/all")
        } catch (error) {
            handleError(error)
        }
    }

    static async getMovie(id: string) {
        try {
            return axios.get<MovieItemDto>("movie?movieId=" + id)
        } catch (error) {
            handleError(error)
        }
    }

    static async uploadMovie(file: File, id: number, onUploadProgress: any) {
        let formData = new FormData();

        formData.append("file", file);
        try {
            return axios.post(
                "movie/upload?movieId=" + id,
                formData,
                {
                    headers: {
                        "Content-Type": "multipart/form-data",
                    },
                    onUploadProgress
                }
            );
        } catch (error) {
            handleError(error)
        }
    };
}