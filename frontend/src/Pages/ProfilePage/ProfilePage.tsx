import React from "react";
import HomePageComponent from "../../Components/HomePage/HomePageComponent";
import {useAuth} from "../../Context/useAuth";

type Props = {};

const ProfilePage = (props: Props) => {

    const { user } = useAuth();

    return (
        <>
            <h1>{user?.username}</h1>
        </>
    );
};

export default ProfilePage;
