import { useIntl } from 'react-intl'
import { PageTitle } from '../../../_metronic/layout/core'
import { ShowComponent } from '../../modules/show/show-component'
import { ShowListModel } from '../../../models/show/show-list-model';
import { get, post } from '../../api/make-api-request-authorized';
import { FC, useEffect, useState } from 'react';
import ServiceResponse, { ServiceResponseWithoutData } from '../../../models/service-response';
import VoteSeasonDetailDto, { VoteSeasonDetailShowDto } from '../../../models/admin/votings/get-all-voting-seasons-response-model';
import { WithChildren } from '../../../_metronic/helpers';
import { toast } from 'react-toastify';

const PopularShowsPage = () => {
    const intl = useIntl()
    const [currentVotings, setCurrentVotings] = useState<VoteSeasonDetailDto[]>([]);
    const [refreshData, setRefreshData] = useState<boolean>(false);
    const [isLoading, setIsLoading] = useState(false)

    const getCurrentVotings = () => {
        setIsLoading(true);
        get<ServiceResponse<VoteSeasonDetailDto[]>>("/votings").then((resolve) => {
            setCurrentVotings(resolve.data.data);
            setIsLoading(false);
        }).then((err) => {
        })
    }

    useEffect(() => {
        document.title = "Koda Tracker - Popular Shows Votings";
        getCurrentVotings();
    }, []);

    useEffect(() => {
        if (refreshData) {
            getCurrentVotings();
            setRefreshData(false);
        }
    }, [refreshData]);


    return (
        <>
            <PageTitle breadcrumbs={[]}>Popular Shows</PageTitle>
            {
                currentVotings.length > 0 &&
                currentVotings.map((voting) => {
                    return (<VotingSeason key={voting.id} data={voting} setRefreshData={setRefreshData} />);
                })
            }
        </>
    )
}

type Props = {
    data: VoteSeasonDetailDto,
    setRefreshData: any
}
const VotingSeason: FC<Props & WithChildren> = ({ data, setRefreshData }) => {
    const voteStatusChanged = (id: string, onFinishHandler: any) => {
        if (!data.isFinished) {
            post<ServiceResponseWithoutData>("/votings/vote-for-show", {
                votingSeasonId: data.id,
                showId: id
            }).then((resolve) => {
                if (resolve.data.isSuccesfull) {
                    setRefreshData(true);
                    onFinishHandler();
                }
            }).catch((err) => {

            });
        }
        else {
            toast("Voting finished")
        }
    }

    const renderBullets = () => {
        const bullets = [];
        const totalPageCount = Math.ceil(data.shows.length / 4);
        for (let index = 0; index < totalPageCount; index++) {
            bullets.push(<><li key={index} data-bs-target={"#votingSeason_" + data.id} data-bs-slide-to={index} className={"ms-1" + (index == 0 ? " active" : "")}></li></>)
        }
        return (bullets);
    }

    const renderShows = () => {
        const bullets = [];
        const totalPageCount = Math.ceil(data.shows.length / 4);
        for (let index = 0; index < totalPageCount; index++) {
            const shows = data.shows.slice(index * 4, (index * 4) + 4);
            bullets.push(
                <>
                    <div className={"carousel-item" + (index == 0 ? " active" : "")}>
                        <div className='row'>
                            {
                                shows.map((show) => {
                                    return (<VotingSeasonShowComponent key={show.id} data={show} onVoted={voteStatusChanged} selected={show.selectedByCurrentUser} isFinished={data.isFinished} />);
                                })
                            }
                        </div>
                    </div>
                </>
            )
        }
        return (bullets);
    }

    return (
        <>
            <div id={"votingSeason_" + data.id} className="carousel carousel-custom slide" data-bs-ride="carousel" data-bs-interval="8000">
                <div className="d-flex align-items-center justify-content-between flex-wrap">
                    <span className="fs-4 fw-bold pe-2">{data.name}<div className="fs-7 text-muted fw-semibold mt-1">Total {data.totalVote} Votes</div></span>

                    <ol className="p-0 m-0 carousel-indicators carousel-indicators-dots">
                        {renderBullets()}
                    </ol>
                </div>

                <div className="carousel-inner pt-8">
                    {renderShows()}
                </div>
            </div>
        </>
    );
}

type ShowProps = {
    data: VoteSeasonDetailShowDto,
    onVoted: any,
    selected: boolean,
    isFinished: boolean
}
const VotingSeasonShowComponent: FC<ShowProps & WithChildren> = ({ data, onVoted, selected, isFinished }) => {
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const vote = () => {
        setIsLoading(true);
        onVoted(data.showId, setIsLoading(false));
    }

    return (
        <>
            <div className="col-md-4 col-xs-6 col-xl-3 col-xxl-3 mb-5">
                <div className="card">
                    <div className="card-body d-flex flex-center flex-column py-9 px-5">
                        <div className="mb-5 w-100">
                            <div className="w-100" style={{
                                height: "350px",
                                overflow: "hidden",
                                position: "relative"
                            }}>
                                <img src={data.coverImageUrl} alt="image" className="w-100" style={{
                                    top: "-9999px",
                                    left: "-9999px",
                                    right: "-9999px",
                                    bottom: "-9999px",
                                    margin: "auto"
                                }} />
                            </div>
                        </div>
                        <span className="fs-4 text-gray-800 fw-bold mb-5">{data.name.length > 25 ? data.name.substring(0, 25) + "..." : data.name}</span>
                        {
                            !isFinished &&
                            <>
                                <button className={"btn btn-sm btn-flex btn-center " + (selected ? "btn-success" : "btn-light")} data-kt-indicator={isLoading ? "on" : "off"} data-kt-follow-btn="true" onClick={() => {
                                    vote();
                                }}>
                                    <i className={"ki-duotone follow fs-3 " + (selected ? "ki-check" : "ki-plus")}></i>
                                    <i className="ki-duotone ki-check following fs-3 d-none"></i>
                                    <span className="indicator-label">
                                        {selected ? "Voted" : "Vote"}</span>
                                    <span className="indicator-progress">
                                        Please wait...
                                        <span className="spinner-border spinner-border-sm align-middle ms-2"></span>
                                    </span>
                                </button>
                            </>
                        }
                        {
                            isFinished &&
                            <>
                                <div className="border border-dashed rounded min-w-90px py-3 px-4 mx-2 mb-3">
                                    <div className="fs-6 fw-bold text-gray-700">Vote Count</div>
                                    <div className='text-success text-center'>
                                        {data.totalVote} votes
                                    </div>
                                </div>
                            </>
                        }
                    </div>
                </div>
            </div>
        </>
    );
}

export { PopularShowsPage }
