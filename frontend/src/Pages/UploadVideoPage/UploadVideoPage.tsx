import React, {useState} from "react";
import {MovieService} from "../../Services/MovieService";
import {useForm} from "react-hook-form";
import {yupResolver} from "@hookform/resolvers/yup";
import * as Yup from "yup";
import {toast} from "react-toastify";
import {UserService} from "../../Services/UserService";
import {useNavigate} from "react-router-dom";
import Navbar from "../../Components/Navbar/Navbar";

type CreateMovieForm = {
    name: string;
    description: string;
    trailerUrl: string;
    posterUrl: string;
};

const validation = Yup.object().shape({
    name: Yup.string().required("Введите название фильма"),
    description: Yup.string().required("Введите описание фильма"),
    trailerUrl: Yup.string().required("Можно ссылку на трейлер плиз...").url("Это не ссылка..."),
    posterUrl: Yup.string().required("Можно ссылку на постер плиз...").url("Это не ссылка...")
})

export const UploadVideoPage = () => {

    const navigate = useNavigate();

    const [movieId, setMovieId] = useState<number>(-1)
    const [currentFile, setCurrentFile] = useState<File>();
    const [progress, setProgress] = useState<number | null>(null);
    const [message, setMessage] = useState<string>("");

    const selectFile = (event: React.ChangeEvent<HTMLInputElement>) => {
        const {files} = event.target;
        const selectedFiles = files as FileList;
        setCurrentFile(selectedFiles?.[0]);
        setProgress(null);
    };

    const {
        register,
        handleSubmit,
        formState: {errors}
    } = useForm<CreateMovieForm>({resolver: yupResolver(validation)})

    const sendMovieDataToServer = (form: CreateMovieForm) => {
        if (movieId < 0) {
            MovieService.movieAddNew(form.name, form.description, form.trailerUrl, form.posterUrl)
                .then(r => {
                        if (r?.data != null) {
                            setMovieId(r.data.id)
                        } else {
                            toast.error("Что-то пошло нет так.")
                        }
                    }
                )
        } else {
            toast.error("Hmm...")
        }
    }

    const saveToFavourite = () => {
        UserService.userPinMovieAPI(movieId).then(r => {
            if (r?.status == 200) {
                toast.success("Готово")
                navigate("/profile")
            }
        }).catch(() => {
        })
    }

    const upload = () => {
        setProgress(0);
        if (!currentFile) return;

        MovieService.uploadMovie(currentFile, movieId, (event: any) => {
            setProgress(Math.round((100 * event.loaded) / event.total));
        })
            .then((response) => {
                setMessage(response?.data.message);
                saveToFavourite()
            })
            .catch((err) => {
                setProgress(null);
                if (err.response && err.response.data && err.response.data.message) {
                    setMessage(err.response.data.message);
                } else {
                    setMessage("Could not upload the File!");
                }

                setCurrentFile(undefined);
            });
    };

    const labelStyle = "block text-l text-gray-900 font-bold"
    const inputStyle = "bg-white border border-gray-300 text-gray-900 rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5"

    const MovieInfoForm =
        <div
            className="w-full bg-gray-50 rounded-lg shadow md:mb-5 sm:max-w-md xl:p-0">
            <div className="space-y-6 p-8">
                <h1 className="text-2xl font-bold leading-tight tracking-tight text-gray-900">
                    Загрузить свой фильм
                </h1>
                <h1 className="leading-tight tracking-tight text-gray-900 md:text-l">
                    Заполните сначала информацию о фильме
                </h1>
                <form
                    className="space-y-3"
                    onSubmit={handleSubmit(sendMovieDataToServer)}>
                    <div>
                        <label className={labelStyle}>
                            Название фильма
                        </label>
                        <input
                            type="text"
                            id="name"
                            className={inputStyle}
                            placeholder="Название"
                            {...register("name")}
                            disabled={movieId > 0}
                        />
                        {errors.name ? (<p className="text-black">{errors.name.message}</p>) : ("")}
                    </div>
                    <label className={labelStyle}>
                        Описание фильма
                    </label>
                    <textarea
                        id="description"
                        className={inputStyle}
                        placeholder="Описание"
                        {...register("description")}
                        disabled={movieId > 0}
                    />
                    {errors.description ? (<p className="text-black">{errors.description.message}</p>) : ("")}
                    <label className={labelStyle}>
                        Ссылка на трейлер
                    </label>
                    <input
                        type="text"
                        id="trailerUrl"
                        className={inputStyle}
                        placeholder="Трейлер"
                        {...register("trailerUrl")}
                        disabled={movieId > 0}
                    />
                    {errors.trailerUrl ? (<p className="text-black">{errors.trailerUrl.message}</p>) : ("")}
                    <label className={labelStyle}>
                        Ссылка на постер
                    </label>
                    <input
                        type="text"
                        id="posterUrl"
                        className={inputStyle}
                        placeholder="Постер"
                        {...register("posterUrl")}
                        disabled={movieId > 0}
                    />
                    {errors.posterUrl ? (<p className="text-black">{errors.posterUrl.message}</p>) : ("")}
                    <button
                        type="submit"
                        className="w-full text-white bg-green-700 hover:bg-primary-700 focus:ring-4 focus:outline-none focus:ring-primary-300 font-medium rounded-lg text-sm px-5 py-2.5 text-center hover:font-bold"
                        hidden={movieId > 0}
                        disabled={movieId > 0}
                    >
                        Сохранить и пререйти к загрузке
                    </button>
                </form>
            </div>
        </div>

    const MovieSelectAndUploadVideo =
        (
            <div
                className="w-full bg-gray-50 rounded-lg shadow md:mb-20 sm:max-w-md xl:p-0">
                <div className="space-y-1 p-8">
                    <label className={labelStyle}>
                        Загрузить видео
                    </label>
                    <input
                        className="block w-full text-sm text-gray-900 border border-gray-300 rounded-lg cursor-pointer bg-gray-50 focus:outline-none file:bg-green-700 file:text-white file:rounded-l file:border-none file:hover:font-bold file:py-2 file:px-1"
                        id="file_input" type="file" accept="video/*"
                        onChange={selectFile}
                        disabled={progress != null}
                    />
                    <button
                        type="submit"
                        className="w-full text-white bg-green-700 hover:bg-primary-700 focus:ring-4 focus:outline-none focus:ring-primary-300 font-medium rounded-lg text-sm px-5 py-2.5 text-center hover:font-bold"
                        disabled={progress != null || !currentFile}
                        onClick={upload}
                    >
                        {!progress ? "Загрузить" : String(progress) + "%"}
                    </button>
                </div>
            </div>
        )


    return (
        <>
            <Navbar/>
            <section className="bg-white">
                <div className="flex flex-col items-center justify px-6 py-8 mx-auto my-20">
                    {MovieInfoForm}
                    {movieId > 0 ? MovieSelectAndUploadVideo : <></>}
                </div>
            </section>
        </>
    )
}