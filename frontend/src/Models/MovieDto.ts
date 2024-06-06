export type MovieDto = {
    id: number,
    name: string,
    imageUrl: string,
    description: string
}

export type CreateMovieDto = {
    id: number
}

export type MovieItemDto = {
    id: number,
    name: string,
    imageUrl: string | null,
    status: string,
    description: string | null,
    trailerUrl: string | null
}