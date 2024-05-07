// @ts-nocheck
import { FC, useEffect, useState } from "react"
import { PageTitle } from "../../../../_metronic/layout/core"
import { AsyncPaginate } from "react-select-async-paginate";
import Select, { GroupBase, MultiValue, OptionsOrGroups, ThemeConfig, components } from 'react-select'
import { Category } from "../../../../models/enums/category-enum";
import { ShowListRequestModel } from "../../../../models/show/show-list-request-model";
import { getAllShows } from "../../../api/show/show-api";
import CreateNewVotingRequestModel, { CreateNewVotingRequestShowModel } from "../../../../models/admin/votings/create-new-voting-request-model";
import Flatpickr from "react-flatpickr";
import { post } from "../../../api/make-api-request-authorized";
import ServiceResponse, { ServiceResponseWithoutData } from "../../../../models/service-response";
import { useNavigate } from "react-router-dom";

const AdminVotingCreatePage = () => {
    const navigator = useNavigate();
    const [refreshData, setRefreshData] = useState(false);
    const [votingName, setVotingName] = useState("");
    const [isLoading, setIsLoading] = useState(false);
    const [currentCategory, setCurrentCategory] = useState<Category>(Category.Anime);
    const [addedShows, setAddedShows] = useState<CreateNewVotingRequestShowModel[]>([]);
    const [search, setSearch] = useState("");
    const [notSelected, setNotSelected] = useState(true);
    const [selectedShow, setSelectedShow] = useState("");
    const [selectedShowName, setSelectedShowName] = useState("");
    const [displayOrder, setDisplayOrder] = useState(1);
    const [formIsValid, setFormIsValid] = useState(false);
    const [dateState, setDateState] = useState<{ startDate: Date | undefined, endDate: Date | undefined }>({
        startDate: undefined,
        endDate: undefined
    });

    useEffect(() => {
        setAddedShows([]);
    }, [currentCategory])

    useEffect(() => {
        let isFormValid = true;
        if (votingName == "") {
            isFormValid = false;
            document.getElementById("txtVotingName")!.style.borderColor = "#FF3767";
        }
        else {
            document.getElementById("txtVotingName")!.style.borderColor = "#00A261";
        }

        if (dateState.startDate == dateState.endDate || dateState.startDate == undefined || dateState.endDate == undefined) {
            isFormValid = false;
            document.getElementById("votingDateRangePicker")!.style.borderColor = "#FF3767";
        }
        else {
            document.getElementById("votingDateRangePicker")!.style.borderColor = "#00A261";
        }

        if (addedShows.length <= 1) {
            isFormValid = false;
        }
        else {
            addedShows.map((show) => {
                var sameDisplayOrderExist = addedShows.filter(f => f.displayOrder == show.displayOrder && show.id != f.id);
                if (sameDisplayOrderExist.length > 0) {
                    isFormValid = false;
                    document.getElementById("showDisplayOrder_" + show.id)!.style.borderColor = "#FF3767";
                    sameDisplayOrderExist.map((sameDisplayOrder) => {
                        document.getElementById("showDisplayOrder_" + sameDisplayOrder.id)!.style.borderColor = "#FF3767";
                    })
                }
                else {
                    document.getElementById("showDisplayOrder_" + show.id)!.style.borderColor = "#363843";
                }
            });
        }

        setFormIsValid(isFormValid);
    }, [votingName, dateState, addedShows])

    const loadOptions = async (search: string, page: number) => {
        const request = new ShowListRequestModel();
        request.Search = search;
        request.pageIndex = page;
        request.pageSize = 50;
        request.Categories = [currentCategory];
        const response = await getAllShows(request);
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

    type AdditionalType = {
        page: number;
    };

    const defaultAdditional: AdditionalType = {
        page: 1
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

    const saveVoting = () => {
        if (formIsValid) {
            var request = new CreateNewVotingRequestModel;
            request.name = votingName;
            request.startDate = dateState.startDate!;
            request.endDate = dateState.endDate!;
            request.shows = addedShows;
            post<ServiceResponseWithoutData>("/votings/create-voting-season", request).then((resolve) => {
                if (resolve.data.isSuccesfull){
                    navigator("/admin/votings");
                }
                else{
                    alert(resolve.data.errorMessage);
                }
            }).catch((err) => {
                alert(err);
            });
        }
    }

    return (
        <>
            <PageTitle breadcrumbs={[]}>Create New Voting</PageTitle>
            <div className='row g-5 g-xl-8'>
                <div className="col-md-12">
                    <label className="form-label">Voting Name</label>
                    <input type="text" className="form-control form-control-solid" id="txtVotingName" value={votingName} onChange={(e) => setVotingName(e.target.value)} placeholder="Voting Name" />
                </div>
                <div className="col-md-12">
                    <label className="form-label">Voting Date Range</label>
                    <Flatpickr
                        id="votingDateRangePicker"
                        onChange={([startDate, endDate]) => {
                            setDateState({ startDate, endDate });
                        }}
                        options={{
                            mode: "range"
                        }}
                        className='form-control form-control-solid'
                        placeholder='Pick date'
                    />
                </div>
                <div className="col-md-12">
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
                            option: (base, props) => ({
                                ...base,
                                backgroundColor: props.isSelected ? "#fff" : "#1B1C22"
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
                        defaultValue={{ value: "1", label: "Anime" }}
                        onChange={(value) => {
                            setCurrentCategory(parseInt(value!.value) as Category);
                        }}
                        placeholder='Select an option'
                    />
                </div>
                <div className="col-md-12">
                    <table className="table table-responsive table-bordered mb-0">
                        <thead>
                            <tr>
                                <th>Name</th>
                                <th>Display Order</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            {addedShows.map((show, index) => {
                                return (
                                    <>
                                        <tr key={show.id}>
                                            <td>
                                                <span className="text-center">{show.name}</span>
                                            </td>
                                            <td>
                                                <span className="text-center">
                                                    <input
                                                        type="number"
                                                        className="form-control"
                                                        id={"showDisplayOrder_" + show.id}
                                                        key={"showDisplayOrder_" + show.id}
                                                        defaultValue={show.displayOrder}
                                                        onBlur={(e) => {
                                                            const value = e.target.value;
                                                            if (value != undefined && value != null) {
                                                                var val = parseInt(value);
                                                                var oldShows = JSON.parse(JSON.stringify(addedShows)) as CreateNewVotingRequestShowModel[];
                                                                oldShows.find(f => f.id == show.id)!.displayOrder = val;
                                                                setAddedShows(oldShows);
                                                            }
                                                        }} />
                                                </span>
                                            </td>
                                            <td>
                                                <button className="btn btn-danger btn-icon me-2" onClick={() => {
                                                    setAddedShows(addedShows.filter(f => f.id != show.id));
                                                }}><i className="ki-outline ki-trash fs-1"></i></button>
                                            </td>
                                        </tr>
                                    </>);
                            })}
                            {
                                <>
                                    <tr>
                                        <td>
                                            <AsyncPaginate
                                                key={currentCategory + "_" + displayOrder}
                                                className='react-select-styled react-select-solid react-select-lg'
                                                classNamePrefix='react-select'
                                                loadOptions={loadPageOptions}
                                                isMulti={false}
                                                isLoading={isLoading}
                                                closeMenuOnSelect={false}
                                                closeMenuOnScroll={false}
                                                isClearable={false}
                                                isSearchable={true}
                                                loadOptionsOnMenuOpen={true}
                                                styles={{
                                                    control: (baseStyles, state) => ({
                                                        ...baseStyles,
                                                        backgroundColor: "#1B1C22",
                                                        border: "none",
                                                        lineHeight: "2.4"
                                                    }),
                                                    option: (base, props) => ({
                                                        ...base,
                                                        backgroundColor: props.isSelected ? "#fff" : "#1B1C22"
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
                                                    setSelectedShow(value!.value);
                                                    setSelectedShowName(value!.label);
                                                    setNotSelected(false);
                                                }}
                                                placeholder='Select an option'
                                                additional={{
                                                    page: 0,
                                                }} />
                                        </td>
                                        <td>
                                            <button className="btn btn-icon btn-success me-2" disabled={notSelected} onClick={() => {
                                                var existedShow = addedShows.filter(f => f.id == selectedShow);
                                                if (existedShow.length > 0) {
                                                    alert("Show already added");
                                                    return;
                                                }
                                                var newShow = new CreateNewVotingRequestShowModel;
                                                newShow.id = selectedShow;
                                                newShow.displayOrder = displayOrder;
                                                newShow.name = selectedShowName;
                                                setAddedShows([...addedShows, newShow]);
                                                setDisplayOrder(displayOrder + 1);
                                            }}>
                                                <i className="ki-outline ki-plus fs-1"></i>
                                            </button>
                                        </td>
                                    </tr>
                                </>
                            }
                        </tbody>
                    </table>
                </div>
                <div className="col-md-12">
                    <button type="button" className="btn btn-success w-100" disabled={!formIsValid} onClick={() => {
                        saveVoting();
                    }}>Save</button>
                </div>
            </div>
        </>
    )
}

export { AdminVotingCreatePage }
