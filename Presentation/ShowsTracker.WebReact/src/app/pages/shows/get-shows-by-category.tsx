import { useIntl } from 'react-intl'
import { PageTitle } from '../../../_metronic/layout/core'
import { ShowComponent } from '../../modules/show/show-component'
import { ShowListModel } from '../../../models/show/show-list-model';
import { useParams } from 'react-router-dom';
import { getAllShows } from '../../api/show/show-api';
import { ShowListRequestModel } from '../../../models/show/show-list-request-model';
import { Category } from '../../../models/enums/category-enum';
import { Component, useEffect, useRef, useState } from 'react';
import { Button } from 'react-bootstrap';
import { Status } from '../../../models/enums/status-enum';
import Select, { GroupBase, MultiValue, OptionsOrGroups, ThemeConfig, components } from 'react-select'
import { AsyncPaginate, Response } from "react-select-async-paginate";
import { getAllGenres } from '../../api/genre/genre-api';
import { GetAllGenresRequestModel } from '../../../models/genre/get-all-genres-request-model';
import { GetAllGenresResponseModel } from '../../../models/genre/get-all-genres-response-model';

const CategoryShowsPage = () => {
    const [shows, setShows] = useState<ShowListModel[]>([]);
    const [currentCategory, setCurrentCategory] = useState(Category.None);
    const [currentGenres, setCurrentGenres] = useState<string[]>([]);
    const [currentStatuses, setCurrentStatuses] = useState<Status[]>([]);
    const [currentStartRating, setCurrentStartRating] = useState(0);
    const [currentEndRating, setCurrentEndRating] = useState(10);
    const [currentPageIndex, setCurrentPageIndex] = useState(0);
    const [currentPageSize, setCurrentPageSize] = useState(20);
    const [currentSearchQuery, setCurrentSearchQuery] = useState<string>("");
    const [selectedGenres, setSelectedGenres] = useState<MultiValue<{ label: string, value: string }>>()
    const [selectedStatuses, setSelectedStatuses] = useState<MultiValue<{ label: string, value: string }>>()
    const [totalPage, setTotalPage] = useState(0);
    const [refreshData, setRefreshData] = useState(false);
    const [isLoading, setIsLoading] = useState(false);
    let { categoryId } = useParams();

    useEffect(() => {
        var categoryNumber = categoryId ? parseInt(categoryId) : 0;
        var currentCategory = categoryNumber as Category;
        setCurrentCategory(currentCategory);
        document.title = "Koda Tracker - " + (
            currentCategory == Category.Anime ? " Anime" :
                currentCategory == Category.Manga ? " Manga" :
                    currentCategory == Category.Manhwa ? " Manhwa" :
                        currentCategory == Category.Manhua ? " Manhua" :
                            currentCategory == Category.Novel ? "Novel" : "");
    }, [categoryId]);

    useEffect(() => {
        if (currentCategory > 0) {
            setCurrentPageIndex(0);
            setCurrentEndRating(10);
            setSelectedGenres([]);
            setCurrentGenres([]);
            setCurrentSearchQuery("");
            setCurrentStartRating(0);
            setCurrentStatuses([]);
            setSelectedStatuses([]);
            setRefreshData(true);
        }
    }, [currentCategory]);

    useEffect(() => {
        if (refreshData) {
            search();
        }
    }, [refreshData]);

    const search = () => {
        var requestData = new ShowListRequestModel();
        requestData.Search = currentSearchQuery;
        requestData.Categories = [currentCategory];
        requestData.pageIndex = currentPageIndex;
        requestData.pageSize = currentPageSize;
        requestData.Statuses = currentStatuses;
        requestData.Genres = currentGenres;
        requestData.StartRating = currentStartRating;
        requestData.EndRating = currentEndRating;
        getAllShows(requestData).then((resolve) => {
            setShows(resolve.data.data.data);
            setTotalPage(resolve.data.data.totalPage);
            setRefreshData(false);
        });
    }

    const renderPaginationButtons = () => {
        var buttons = [];
        if (totalPage > 10) {
            var firstIndex = currentPageIndex > 5 ? currentPageIndex - 4 : 0;
            var lastIndex = currentPageIndex > 5 ? currentPageIndex + 4 > totalPage ? totalPage : currentPageIndex + 4 : 10;
            for (var i = firstIndex; i <= lastIndex; i++) {
                var pageName = "page_" + i;
                buttons.push(
                    <>
                        <li key={pageName} className={"page-item " + pageName + " " + (i == currentPageIndex && "active")}><button className="page-link" value={i} onClick={(e) => {
                            setCurrentPageIndex(parseInt(e.currentTarget.value));
                            setRefreshData(true);
                        }}>{(i + 1)}</button></li>
                    </>);
            }
        }
        else {
            for (var i = 0; i <= totalPage; i++) {
                var pageName = "page_" + i;
                buttons.push(
                    <>
                        <li key={pageName} className={"page-item " + (i == currentPageIndex && "active")}><button className="page-link" value={i} onClick={(e) => {
                            setCurrentPageIndex(parseInt(e.currentTarget.value));
                            setRefreshData(true);
                        }}>{(i + 1)}</button></li>
                    </>);
            }
        }
        return (<>{buttons}</>);
    }

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

    return (
        <>
            <PageTitle breadcrumbs={[]}>{
                currentCategory == Category.Anime ? " Anime" :
                    currentCategory == Category.Manga ? " Manga" :
                        currentCategory == Category.Manhwa ? " Manhwa" :
                            currentCategory == Category.Manhua ? " Manhua" :
                                currentCategory == Category.Novel ? "Novel" :
                                    ""
            }
            </PageTitle>
            <div className='row'>
                <div className='col-md-3'>
                    <div>
                        <div className="mb-10">
                            <label className="form-label">Search by keyword</label>
                            <input type="text" className="form-control form-control-solid" value={currentSearchQuery} onChange={(e) => setCurrentSearchQuery(e.target.value)} placeholder="Search by keyword" />
                        </div>
                    </div>
                </div>
                <div className='col-md-3'>
                    <div>
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
                </div>
                <div className='col-md-2'>
                    <div>
                        <div className="mb-10">
                            <label className="form-label">Status</label>
                            <Select
                                className='react-select-styled react-select-solid react-select-lg'
                                classNamePrefix='react-select'
                                options={
                                    [
                                        { value: "1", label: "Ongoing" },
                                        { value: "2", label: "Done" },
                                        { value: "3", label: "Onbreak" },
                                    ]
                                }
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
                                value={selectedStatuses}
                                onChange={(value) => {
                                    setSelectedStatuses(value);
                                    setCurrentStatuses(value.map((status) => {
                                        return parseInt(status.value) as Status;
                                    }));
                                }}
                                placeholder='Select an option'
                            />
                        </div>
                    </div>
                </div>
                <div className='col-md-1'>
                    <div>
                        <div className="mb-10">
                            <label className="form-label">Min Rate</label>
                            <input type="number" className="form-control form-control-solid" value={currentStartRating} defaultValue={0} min={0} max={10} onChange={(e) => {
                                const val = parseInt(e.target.value);
                                if (val > 10) {
                                    e.target.value = "10";
                                }
                                else if (val < 0) {
                                    e.target.value = "0";
                                }
                                else {
                                    setCurrentStartRating(val);
                                }
                            }} />
                        </div>
                    </div>
                </div>
                <div className='col-md-1'>
                    <div>
                        <div className="mb-10">
                            <label className="form-label">Max Rate</label>
                            <input type="number" className="form-control form-control-solid" value={currentEndRating} defaultValue={10} min={0} max={10} onChange={(e) => {
                                const val = parseInt(e.target.value);
                                if (val > 10) {
                                    e.target.value = "10";
                                }
                                else if (val < 0) {
                                    e.target.value = "0";
                                }
                                else {
                                    setCurrentEndRating(val);
                                }
                            }} />
                        </div>
                    </div>
                </div>
                <div className='col-md-2'>
                    <div>
                        <div className="mb-10">
                            <label className="form-label">&#160;</label>
                            <button className="btn btn-success form-control" onClick={() => {
                                setCurrentPageIndex(0);
                                setRefreshData(true);
                            }}>Search</button>
                        </div>
                    </div>
                </div>
            </div >
            <div className='row mb-10'>
                {shows.map((show: ShowListModel, index) => {
                    return (<ShowComponent key={show.id} series={show} />);
                })}
            </div>
            <div>
                <ul className="pagination">
                    <li className={"page-item previous" + (currentPageIndex == 0 ? "disabled" : "")}>
                        <button className="page-link" onClick={() => {
                            if (currentPageIndex > 1) {
                                setCurrentPageIndex(currentPageIndex - 1);
                                setRefreshData(true);
                            }
                        }}>
                            <i className="previous"></i>
                        </button>
                    </li>
                    {renderPaginationButtons()}
                    <li className={"page-item next" + (currentPageIndex == totalPage ? "disabled" : "")} onClick={() => {
                        if (currentPageIndex < totalPage) {
                            setCurrentPageIndex(currentPageIndex + 1);
                            setRefreshData(true);
                        }
                    }}>
                        <button className="page-link"><i className="next"></i></button>
                    </li>
                </ul>
            </div>
        </>
    )
}

export { CategoryShowsPage }
