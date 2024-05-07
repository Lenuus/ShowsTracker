// @ts-nocheck
import { useEffect, useState } from "react";
import ReactDomServer from 'react-dom/server';
import { Grid } from './grid-content-preview/js/grid.js';
import { Category } from "../../../models/enums/category-enum.js";
import { GetAllGenresRequestModel } from "../../../models/genre/get-all-genres-request-model.js";
import { getAllGenres } from "../../api/genre/genre-api.js";
import Select, { MultiValue } from "react-select";
import { AsyncPaginate } from "react-select-async-paginate";
import { post } from "../../api/make-api-request-authorized.js";
import ServiceResponse from "../../../models/service-response.js";
import { ShowListModel } from "../../../models/show/show-list-model.js";

const RandomShowsPage = () => {
    const [currentGenres, setCurrentGenres] = useState<string[]>([]);
    const [selectedCategories, setSelectedCategories] = useState<Category>(Category.None);
    const [selectedGenres, setSelectedGenres] = useState<MultiValue<{ label: string, value: string }>>()
    const [isLoading, setIsLoading] = useState(false);
    const [isDisabled, setIsDisabled] = useState(true);
    const imagePlaces = [2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 13, 14, 15]

    useEffect(() => {
        setIsDisabled(!(selectedCategories != Category.None && selectedGenres?.length > 0))
    }, [selectedGenres, selectedCategories])

    type AdditionalType = {
        page: number;
    };

    const defaultAdditional: AdditionalType = {
        page: 1
    };

    const loadOptions = async (search: string, page: number) => {
        const request = new GetAllGenresRequestModel();
        request.search = search;
        request.pageIndex = page;
        request.pageSize = 50;
        const response = await getAllGenres(request);
        const data = response.data.data;
        const hasMore = data.totalPage > data.currentPage;
        const options = data.data.map((genre) => {
            return { label: genre.name, value: genre.id }
        });
        return {
            options: options,
            hasMore
        };
    };

    const loadPageOptions = async (
        q: string,
        prevOptions: unknown,
        { page }: AdditionalType
    ) => {
        const { options, hasMore } = await loadOptions(q, page);

        return {
            options,
            hasMore,
            additional: {
                page: page + 1
            }
        };
    };

    const find = () => {
        post<ServiceResponse<ShowListModel[]>>("/show/find-your-taste", { category: selectedCategories, genres: selectedGenres?.map((genre) => { return genre.value; }) }).then((resolve) => {
            if (resolve.data.isSuccesfull) {
                document.getElementById("root")!.innerHTML = ReactDomServer.renderToString(renderPage(resolve.data.data));
                new Grid(document.querySelector('.grid'));
                let followButtons = document.getElementsByClassName("btnFollow");
                for (let index = 0; index < followButtons.length; index++) {
                    const element = followButtons[index];
                    element.addEventListener("click", (e) => {
                        let id = e.target.dataset["id"];
                        post<ServiceResponse<boolean>>("/user-show/add-show", { showId: id }).then((resolve) => {
                            e.target.innerText = "Followed";
                        });
                        e.target.disabled = true;
                    });
                }
            }
        })
    }

    function getRandomInt(max: number) {
        return Math.floor(Math.random() * max);
    }

    function renderPage(data: ShowListModel[]) {
        const previews = [];
        const fillers = [];
        const randomPositions: number[] = [];
        for (let index = 0; index < data.length; index++) {
            let randomPosition = 0;
            do {
                randomPosition = getRandomInt(imagePlaces.length)
            }
            while (randomPositions.includes(randomPosition));
            randomPositions.push(randomPosition);
            const element = data[index];
            previews.push(
                <a href={"#preview-" + index}
                    className={"grid__item pos-" + imagePlaces[randomPosition]}
                    data-bs-toggle="modal"
                    data-bs-target={"#kt_modal_" + index}
                    data-title={element.name}>
                    <div className="grid__item-img" style={{ backgroundImage: "url(" + element.coverImageUrl + ")" }}></div>
                </a>
            );
            fillers.push(
                <>
                    <div className="preview__item" id={"preview-" + index}>
                        <button className="preview__item-back unbutton"><span></span></button>
                        <div className="preview__item-imgwrap">
                            <div className="preview__item-img" style={{ backgroundImage: "url(" + element.coverImageUrl + ")" }}></div>
                        </div>
                        <h2 className="preview__item-title"></h2>
                    </div>
                    <div className="modal fade" tabIndex={-1} id={"kt_modal_" + index}>
                        <div className="modal-dialog modal-lg">
                            <div className="modal-content">
                                <div className="modal-header">
                                    <h5 className="modal-title">{element.name}</h5>
                                    <div
                                        className="btn btn-icon btn-sm btn-active-light-primary ms-2"
                                        data-bs-dismiss="modal"
                                        aria-label="Close"
                                    >
                                        <i className="las la-times"></i>
                                    </div>
                                </div>
                                <div className="modal-body row">
                                    <div className="col-md-6">
                                        <img className="w-100" src={element.coverImageUrl} />
                                    </div>
                                    <div className="col-md-6">
                                        <p><b>Name: {element.name}</b></p>
                                        <p><b>Genres: {element.genres.map((genre) => { return genre.name }).join(", ")}</b></p>
                                        <p><b>Description: {element.name}</b></p>
                                    </div>
                                </div>
                                <div className="modal-footer">
                                    <button
                                        type="button"
                                        className="btn btn-light"
                                        data-bs-dismiss="modal"
                                    >
                                        Close
                                    </button>
                                    <button type="button" className="btn btn-success btnFollow" data-id={element.id}>
                                        Follow
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                </>

            );
        }
        return (
            <main>
                <link rel="stylesheet" href="/grid-content-preview/style.css"></link>
                <div className="frame">
                    <div className="frame__title-wrap">
                        <a href="/find-your-taste">Return To Page</a>
                    </div>
                </div>
                <div className="content">
                    <h2 className="content__title">
                        <span className="content__title-line content__title-line--1"></span>
                        <span className="content__title-line content__title-line--2"></span>
                    </h2>
                    <div className="grid">
                        {previews}
                    </div>
                    <div className="preview">
                        {fillers}
                    </div>
                </div>
            </main>
        );
    }

    return (
        <>
            <div className="d-flex flex-column flex-center flex-column-fluid">
                <div className="d-flex flex-column flex-center text-center p-10">
                    <div className="card card-flush w-lg-650px py-5">
                        <div className="card-body py-15 py-lg-20">

                            <h1 className="fw-bolder text-gray-900 mb-5">
                                Find Your Taste
                            </h1>

                            <div className="fs-6 mb-8">

                                <div className="mb-10">
                                    <label className="form-label">Category</label>
                                    <Select
                                        className='react-select-styled react-select-solid react-select-lg'
                                        classNamePrefix='react-select'
                                        options={
                                            [
                                                { value: "1", label: "Anime" },
                                                { value: "2", label: "Manga" },
                                                { value: "3", label: "Manhwa" },
                                                { value: "4", label: "Manhua" },
                                                { value: "5", label: "Novel" },
                                            ]
                                        }
                                        isMulti={false}
                                        isLoading={isLoading}
                                        closeMenuOnSelect={true}
                                        closeMenuOnScroll={false}
                                        isClearable={false}
                                        isSearchable={false}
                                        loadOptionsOnMenuOpen={true}
                                        styles={{
                                            control: (baseStyles, state) => ({
                                                ...baseStyles,
                                                backgroundColor: "#1B1C22",
                                                border: "none",
                                                lineHeight: "2.4",
                                            }),
                                            option: (baseStyles, { isFocused, isSelected }) => ({
                                                ...baseStyles,
                                                backgroundColor: isSelected ? "#B5B7C8" : "#1B1C22",
                                            }),
                                        }}
                                        // @ts-ignore
                                        theme={(theme) => ({
                                            ...theme,
                                            colors: {
                                                primary: 'black',
                                                primary25: '#363843',
                                                neutral0: '#1B1C22',
                                                neutral50: '#fff',

                                            },
                                        })}
                                        onChange={(value) => {
                                            setSelectedCategories(parseInt(value!.value) as Category);
                                        }}
                                        placeholder='Select an option'
                                    />
                                </div>
                                <div className="mb-10">
                                    <label className="form-label">Genres</label>
                                    <AsyncPaginate
                                        className='react-select-styled react-select-solid react-select-lg'
                                        classNamePrefix='react-select'
                                        // @ts-ignore
                                        loadOptions={loadPageOptions}
                                        isMulti={true}
                                        isLoading={isLoading}
                                        closeMenuOnSelect={false}
                                        closeMenuOnScroll={false}
                                        isClearable={true}
                                        isSearchable={true}
                                        loadOptionsOnMenuOpen={true}
                                        styles={{
                                            control: (baseStyles, state) => ({
                                                ...baseStyles,
                                                backgroundColor: "#1B1C22",
                                                border: "none",
                                                lineHeight: "2.4"
                                            }),
                                        }}
                                        // @ts-ignore
                                        theme={(theme) => ({
                                            ...theme,
                                            colors: {
                                                primary: 'black',
                                                primary25: '#363843',
                                                neutral0: '#1B1C22',
                                                neutral50: '#fff',
                                            },
                                        })}
                                        value={selectedGenres}
                                        onChange={(value) => {
                                            setSelectedGenres(value);
                                            setCurrentGenres(value.map((genre) => {
                                                return genre.value;
                                            }));
                                        }}
                                        placeholder='Select an option'
                                        additional={{
                                            page: 0,
                                        }} />
                                </div>
                            </div>

                            <div>
                                <button className="btn btn-success" onClick={find} disabled={isDisabled}>Find</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </>
    );
}

export { RandomShowsPage }