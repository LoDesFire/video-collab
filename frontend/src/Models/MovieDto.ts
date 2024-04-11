import {number} from "yup";

export type MovieDto = {
    id: number,
    name: string,
    imageUrl: string,
    description: string
}

export type CreateMovieDto = {
    id: number
}