// @ts-nocheck
import { useIntl } from 'react-intl'
import { PageTitle } from '../../../_metronic/layout/core'
import { useEffect, useState } from 'react';
import { post } from '../../api/make-api-request-authorized';
import ServiceResponse from '../../../models/service-response';
import { MyShowListModel } from '../../../models/my-shows/my-show-list-response-model';
import { MyShowListRequestModel } from '../../../models/my-shows/my-show-list-request-model';
import { MyShowComponent } from '../../modules/show/my-show-component';
import { PagedResponseModel } from '../../../models/paged-response-model';
import { useAuth } from '../../modules/auth';
import { useNavigate } from 'react-router-dom';
import Select, { GroupBase, MultiValue, OptionsOrGroups, ThemeConfig, components } from 'react-select'
import { Category } from '../../../models/enums/category-enum';
import { AsyncPaginate } from 'react-select-async-paginate';
import { GetAllGenresRequestModel } from '../../../models/genre/get-all-genres-request-model';
import { getAllGenres } from '../../api/genre/genre-api';
import { Status } from '../../../models/enums/status-enum';
import { TrackStatus } from '../../../models/enums/track-status-enum';

const MyShowsPage = () => {
    const auth = useAuth();
    const intl = useIntl()
    const [myShows, setMyShows] = useState<MyShowListModel[]>([]);

    const [currentCategory, setCurrentCategory] = useState<Category[]>([]);
    const [currentGenres, setCurrentGenres] = useState<string[]>([]);
    const [currentSearchQuery, setCurrentSearchQuery] = useState<string>("");
    const [currentStatuses, setCurrentStatuses] = useState<Status[]>([]);
    const [currentTrackStatuses, setCurrentTrackStatuses] = useState<TrackStatus[]>([]);

    const [currentPageIndex, setCurrentPageIndex] = useState(0);
    const [currentPageSize, setCurrentPageSize] = useState(12);
    const [totalPage, setTotalPage] = useState(0);
    const [refreshData, setRefreshData] = useState(false);
    const [options, setOptions] = useState([]);
    const [isLoading, setIsLoading] = useState(false);

    const navigate = useNavigate();

    useEffect(() => {
        document.title = "Koda Tracker - My Shows";
        if (!auth.currentUser)
            navigate("/auth/login");
        search();
    }, []);

    useEffect(() => {
        if (refreshData) {
            search();
        }
    }, [refreshData]);

    const search = () => {
        var requestData = new MyShowListRequestModel;
        requestData.pageIndex = currentPageIndex;
        requestData.pageSize = currentPageSize;
        requestData.categories = currentCategory;
        requestData.genres = currentGenres;
        requestData.search = currentSearchQuery;
        requestData.statuses = currentStatuses;
        requestData.trackStatuses = currentTrackStatuses;
        post<ServiceResponse<PagedResponseModel<MyShowListModel>>>("/user-show/get-my-shows", requestData).then((resolve) => {
            setMyShows(resolve.data.data.data);
            setTotalPage(resolve.data.data.totalPage);
            setRefreshData(false);
        });
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

    return (
        <>
            <PageTitle breadcrumbs={[]}>My Shows</PageTitle>

            <div className='row'>
                <div className='col-md-2'>
                    <div>
                        <div className="mb-10">
                            <label className="form-label">Search by keyword</label>
                            <input type="text" className="form-control form-control-solid" value={currentSearchQuery} onChange={(e) => setCurrentSearchQuery(e.target.value)} placeholder="Search by keyword" />
                        </div>
                    </div>
                </div>
                <div className='col-md-2'>
                    <div>
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
                                    setCurrentCategory(value.map((status) => {
                                        return parseInt(status.value) as Category;
                                    }));
                                }}
                                placeholder='Select an option'
                            />
                        </div>
                    </div>
                </div>
                <div className='col-md-2'>
                    <div>
                        <div className="mb-10">
                            <label className="form-label">Genres</label>
                            <AsyncPaginate
                                className='react-select-styled react-select-solid react-select-lg'
                                classNamePrefix='react-select'
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
                            <label className="form-label">Show Status</label>
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
                                    setCurrentStatuses(value.map((status) => {
                                        return parseInt(status.value) as Status;
                                    }));
                                }}
                                placeholder='Select an option'
                            />
                        </div>
                    </div>
                </div>
                <div className='col-md-2'>
                    <div>
                        <div className="mb-10">
                            <label className="form-label">Track Status</label>
                            <Select
                                className='react-select-styled react-select-solid react-select-lg'
                                classNamePrefix='react-select'
                                options={
                                    [
                                        { value: "1", label: "Ongoing" },
                                        { value: "2", label: "OnBreak" },
                                        { value: "3", label: "Dropped" },
                                        { value: "4", label: "Done" },
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
                                    setCurrentTrackStatuses(value.map((status) => {
                                        return parseInt(status.value) as TrackStatus;
                                    }));
                                }}
                                placeholder='Select an option'
                            />
                        </div>
                    </div>
                </div>
                <div className='col-md-2'>
                    <div>
                        <div className="mb-10">
                            <label className="form-label">&#160;</label>
                            <button className="btn btn-success form-control" onClick={() =>{
                                setCurrentPageIndex(0);
                                setRefreshData(true);
                            }}>Search</button>
                        </div>
                    </div>
                </div>
            </div>
            <div className='row'>
                {myShows.map((show) => {
                    return (<MyShowComponent key={show.id} series={show} setRefreshData={setRefreshData} />)
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

export { MyShowsPage }
