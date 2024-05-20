import React from "react";
import HomePageComponent from "../../Components/HomePage/HomePageComponent";
import Navbar from "../../Components/Navbar/Navbar";

type Props = {};

const HomePage = (props: Props) => {
    return (
        <>
            <Navbar/>
            <HomePageComponent/>
        </>
    );
};

export default HomePage;
