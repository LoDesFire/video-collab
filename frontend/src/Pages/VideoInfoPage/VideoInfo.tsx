import React, {useEffect, useState} from "react";
import {useParams} from "react-router-dom";
import {MovieItemDto} from "../../Models/MovieDto";
import Navbar from "../../Components/Navbar/Navbar";
import {MovieService} from "../../Services/MovieService";

export const VideoInfo = () => {
    const {id} = useParams<{ id: string }>();
    const [movie, setMovie] = useState<MovieItemDto | null>(null);
    const [loading, setLoading] = useState(true)
    const [status, setStatus] = useState("")

    useEffect(() => {
        if (id) MovieService.getMovie(id).then(r => {
            if (r?.status == 200) {
                setMovie(r.data)
                switch (r.data.status) {
                    case "ReadyToView":
                        console.log("1")
                        setStatus("Готов к просмотру")
                        break;
                    case "NotUploaded":
                        setStatus("Не загружен")
                        break;
                    case "InQueue":
                        setStatus("В очереди")
                        break;
                    case "Transcoding":
                        setStatus("Обработка")
                        break;
                    case "TranscodingError":
                        setStatus("Ошибка обработки")
                        break;
                    case "StartTranscodingError":
                        setStatus("Не удалось начать обработку")
                        break;
                }
            }
            setLoading(false)
            console.log(movie?.status)
        }).catch(() => {
        })
    }, [id]);

    if (loading) {
        return <div className="text-center mt-10">Загрузка...</div>;
    }

    if (!movie) {
        return <div className="text-center mt-10">Фильм не найден</div>;
    }

    return (
        <>
            <Navbar/>
            <div className="max-w-4xl mx-auto p-4">
                <div className="bg-gray-50 shadow-md rounded-lg overflow-hidden">
                    {movie.imageUrl && (
                        <img src={movie.imageUrl} alt={`${movie.name} Poster`}
                             className="w-full h-64 object-scale-down"/>)}
                    <div className="p-6">
                        <h2 className="text-2xl font-bold mb-2">{movie.name}</h2>
                        <p className="text-gray-700 mb-4">{movie.description}</p>
                        {movie.trailerUrl &&
                            <a
                                href={movie.trailerUrl}
                                target="_blank"
                                rel="noopener noreferrer"
                                className="inline-block px-4 py-2 bg-green-700 text-white font-semibold rounded-lg shadow-md hover:bg-green-800">
                                Трейлер
                            </a>}
                        <p className="text-gray-700 mt-4">Статус: {status}</p>
                    </div>
                </div>
            </div>
        </>
    );
}