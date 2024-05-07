// @ts-nocheck
import { FC, useEffect, useState } from "react"
import { PageTitle } from "../../../../_metronic/layout/core"
import { WithChildren } from "../../../../_metronic/helpers";
import VoteSeasonDetailDto from "../../../../models/admin/votings/get-all-voting-seasons-response-model";
import { deleteRequest, post } from "../../../api/make-api-request-authorized";
import ServiceResponse, { ServiceResponseWithoutData } from "../../../../models/service-response";
import { PagedResponseModel } from "../../../../models/paged-response-model";
import GetAllVotingSeasonsRequestModel from "../../../../models/admin/votings/get-all-voting-seasons-request-model";
import moment from "moment"
import { Link } from "react-router-dom";

const AdminVotingPage = () => {
    const [refreshData, setRefreshData] = useState(false);

    return (
        <>
            <PageTitle breadcrumbs={[]}>Votings Administrate</PageTitle>
            <div className='row g-5 g-xl-8'>
                <div className="col-md-12">
                    <Link to={"/admin/votings/create"} className="btn btn-success w-100">Create New Voting</Link>
                </div>
                <VotingTable refreshData={refreshData} />
            </div>
        </>
    )
}

type Props = {
    refreshData: boolean
}
const VotingTable: FC<Props & WithChildren> = ({ refreshData }) => {
    const [data, setData] = useState<VoteSeasonDetailDto[]>([]);
    const [pageIndex, setPageIndex] = useState(0);
    const [pageSize, setPageSize] = useState(20);

    const getAllVotings = () => {
        var request = new GetAllVotingSeasonsRequestModel;
        post<ServiceResponse<PagedResponseModel<VoteSeasonDetailDto>>>("/votings/list", request).then((resolve) => {
            setData(resolve.data.data.data);
        }).catch((err) => {
        })
    }

    useEffect(() => {
        getAllVotings()
    }, []);

    useEffect(() => {
        if (refreshData) {
            getAllVotings();
        }
    }, [refreshData]);

    const deleteVoting = (id: string) => {
        deleteRequest<ServiceResponseWithoutData>("/votings/delete-voting-season/" + id).then((resolve) => {
            if (resolve.data.isSuccesfull) {
                document.getElementById("voting_" + id)!.outerHTML = "";
            }
        }).catch((err) => {
            alert(err)
        })
    }

    return (
        <>
            {
                data.map((voting) => {
                    return (<>
                        <div className="col-xl-6" id={"voting_" + voting.id}>
                            <div className="card card-xl-stretch mb-xl-8">
                                <div className="card-header align-items-center border-0 mt-4">
                                    <h3 className="card-title align-items-start flex-column">
                                        <span className="fw-bold text-gray-900">{voting.name}</span>
                                        <span className="text-muted mt-1 fw-semibold fs-7">{moment(voting.startDate).format("LL")} - {moment(voting.endDate).format("LL")} - {voting.totalVote} total votes</span>
                                    </h3>
                                    <div className="card-toolbar">
                                        <button type="button" className="btn btn-sm btn-danger btn-icon" onClick={() => {
                                            deleteVoting(voting.id);
                                        }}>
                                            <i className="ki-outline ki-trash fs-1"></i>
                                        </button>
                                    </div>
                                </div>
                                <div className="card-body pt-3">
                                    {voting.shows.map((show) => {
                                        const percent = (show.totalVote / voting.totalVote * 100).toFixed();
                                        return (<>
                                            <div className="d-flex align-items-sm-center mb-7">
                                                <div className="symbol symbol-60px symbol-2by3 me-4">
                                                    <div className="symbol-label" style={{ backgroundImage: "url('" + show.coverImageUrl + "')" }}></div>
                                                </div>
                                                <div className="d-flex flex-row-fluid flex-wrap align-items-center">
                                                    <div className="flex-grow-1 me-2">
                                                        <span className="text-gray-800 fw-bold fs-6">{show.name.length > 25 ? show.name.substring(0, 25) + "..." : show.name}</span>

                                                        <span className="text-muted fw-semibold d-block pt-1">{show.totalVote} total vote</span>
                                                    </div>
                                                    <div className="d-flex flex-column w-50 me-2">
                                                        <div className="d-flex flex-stack mb-2">
                                                            <span className="text-muted me-2 fs-7 fw-bold">
                                                                {percent}%
                                                            </span>
                                                        </div>

                                                        <div className="progress h-6px w-100">
                                                            <div className="progress-bar bg-primary" role="progressbar" style={{ width: percent + "%" }} aria-valuenow={percent} aria-valuemin="0" aria-valuemax="100"></div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </>)
                                    })}
                                </div>
                            </div>
                        </div>
                    </>);
                })
            }
        </>
    );
}

export { AdminVotingPage }
