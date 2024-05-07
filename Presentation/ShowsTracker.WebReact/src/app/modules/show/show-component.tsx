import { FC, RefAttributes, useEffect, useState } from "react"
import { WithChildren } from "../../../_metronic/helpers"
import { ShowListModel } from "../../../models/show/show-list-model"
import { Status } from "../../../models/enums/status-enum"
import { Category } from "../../../models/enums/category-enum"
import { ReleaseType } from "../../../models/enums/release-type-enum"
import { useAuth } from "../auth"
import { useNavigate } from "react-router-dom"
import { post } from "../../api/make-api-request-authorized"
import ServiceResponse from "../../../models/service-response"
import { ShowLinkModal } from "../modal/show-link-modal"
import { ToastContainer, toast } from 'react-toastify';
import { OverlayTrigger, Tooltip } from "react-bootstrap"

type Props = {
  series: ShowListModel
}
const ShowComponent: FC<Props & WithChildren> = ({ series }) => {
  const { currentUser, logout } = useAuth()
  const navigate = useNavigate();
  const [isFollowedByCurrentUser, setIsFollowedByCurrentUser] = useState(series.isFollowedByCurrentUser);
  const [followStateChanged, setFollowStateChanged] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [showLinksModal, setShowLinksModal] = useState(false);
  const [tempIntValue, setTempIntValue] = useState(0);
  const [errorMessage, setErrorMessage] = useState("");
  useEffect(() => {
    if (followStateChanged) {
      if (!currentUser) {
        navigate("/auth/login");
      }
      if (isFollowedByCurrentUser) {
        unfollowShow();
      }
      else {
        followShow();
      }
    }
  }, [followStateChanged])

  const followShow = () => {
    setIsLoading(true);
    post<ServiceResponse<boolean>>("/user-show/add-show", { showId: series.id }).then((resolve) => {
      if (resolve.data.isSuccesfull) {
        setIsFollowedByCurrentUser(!isFollowedByCurrentUser);
        setFollowStateChanged(false);
      }
      setIsLoading(false);
      setErrorMessage("Following")
    }).catch((resolve) => {
      setIsLoading(false);
      setErrorMessage(resolve.response.data.errorMessage);
    });
  }

  const unfollowShow = () => {
    setIsLoading(true);
    post<ServiceResponse<boolean>>("/user-show/drop-show", { showId: series.id }).then((resolve) => {
      if (resolve.data.isSuccesfull) {
        setIsFollowedByCurrentUser(!isFollowedByCurrentUser);
        setFollowStateChanged(false);
      }
      setIsLoading(false);
      setErrorMessage("Dropped")
    }).catch((resolve) => {
      setIsLoading(false);
      setErrorMessage(resolve.response.data.errorMessage);
    });
  }

  useEffect(() => {
    if (errorMessage != undefined && errorMessage != "")
      toast(errorMessage);
  }, [errorMessage]);

  const tooltip = (
    <Tooltip id="tooltip">
      {
        series.status == Status.Ongoing && series.category == Category.Anime &&
        <>
          <div className="border border-dashed rounded min-w-90px py-3 px-4 mx-2 mb-3">
            <div className="fs-6 fw-bold text-gray-700">Broadcast</div>
            <div className={"fw-semibold text-success"}>
              {
                series.releaseGap > 0 ?
                  <>
                    Every
                    {series.releaseGap == 1 ? "" : " " + series.releaseGap}
                    {
                      series.releaseType == ReleaseType.Daily ? " Day" :
                        series.releaseType == ReleaseType.Month ? " Month" :
                          series.releaseType == ReleaseType.Week ? " Week" :
                            ""
                    }</>
                  : "Unknown"}

            </div>
          </div>
        </>
      }
      {
        series.rating > 0 &&
        <>
          <div className="border border-dashed rounded min-w-90px py-3 px-4 mx-2 mb-3">
            <div className="fs-6 fw-bold text-gray-700">Rating</div>
            <div className={"fw-semibold text-success"}>
              {series.rating} / 10
            </div>
          </div>
        </>
      }
      {
        series.genres.length > 0 &&
        <>
          <div className="border border-dashed rounded min-w-90px py-3 px-4 mx-2 mb-3">
            <div className="fs-6 fw-bold text-gray-700">Genres</div>
            <div className={"fw-semibold text-success"}>
              {
                series.genres.map((genre, index) => {
                  if (index == series.genres.length - 1)
                    return genre.name;
                  else
                    return genre.name + ", ";
                })
              }
            </div>
          </div>
        </>
      }
    </Tooltip>
  );
  
  return (
    <>
      <OverlayTrigger placement="left" overlay={tooltip}>
        <div className="col-md-3 col-xxl-3 mb-5">
          <div className="card">
            <div className="ribbon ribbon-triangle ribbon-top-end border-dark" onClick={() => {
              setShowLinksModal(true);
              setTempIntValue(tempIntValue + 1);
            }} style={{ zIndex: "0" }}>
              <div className="ribbon-icon mt-n5 me-n6">
                <i className="bi bi-three-dots fs-2 text-white"></i>
              </div>
            </div>
            <div className="card-body d-flex flex-center flex-column py-9 px-5">
              <div className="mb-5 w-100">
                <div className="w-100" style={{
                  height: "350px",
                  overflow: "hidden",
                  position: "relative"
                }}>
                  <img src={series.coverImageUrl} alt="image" className="w-100" style={{
                    top: "-9999px",
                    left: "-9999px",
                    right: "-9999px",
                    bottom: "-9999px",
                    margin: "auto",
                  }} />
                </div>
              </div>

              <a href="#" className="fs-4 text-gray-800 text-hover-primary fw-bold mb-0">{series.name.length > 25 ? series.name.substring(0, 25) + "..." : series.name}</a>
              <div className="fw-semibold text-gray-500 mb-6">
                {series.category == Category.Anime ? "Anime" :
                  series.category == Category.Manga ? "Manga" :
                    series.category == Category.Manhua ? "Manhua" :
                      series.category == Category.Manhwa ? "Manhwa" :
                        series.category == Category.Novel ? "Novel" :
                          ""}
              </div>
              <div className="d-flex flex-center flex-wrap mb-5">
                {
                  series.status == Status.Ongoing && false && series.category == Category.Anime &&
                  <>
                    <div className="border border-dashed rounded min-w-90px py-3 px-4 mx-2 mb-3">
                      <div className="fs-6 fw-bold text-gray-700">Broadcast</div>
                      <div className={"fw-semibold text-success"}>
                        {
                          series.releaseGap > 0 ?
                            <>
                              Every
                              {series.releaseGap == 1 ? "" : " " + series.releaseGap}
                              {
                                series.releaseType == ReleaseType.Daily ? " Day" :
                                  series.releaseType == ReleaseType.Month ? " Month" :
                                    series.releaseType == ReleaseType.Week ? " Week" :
                                      ""
                              }</>
                            : "Unknown"}

                      </div>
                    </div>
                  </>
                }

                <div className="border border-dashed rounded min-w-90px py-3 px-4 mx-2 mb-3">
                  <div className="fs-6 fw-bold text-gray-700">Status</div>
                  <div className={"fw-semibold " +
                    (series.status == Status.Done ? "text-danger" :
                      series.status == Status.OnBreak ? "text-info" :
                        series.status == Status.Ongoing ? "text-success" :
                          "text-info")}>
                    {series.status == Status.Done ? "Done" :
                      series.status == Status.OnBreak ? "Onbreak" :
                        series.status == Status.Ongoing ? "Ongoing" :
                          "Unknown"}
                  </div>
                </div>

                <div className="border border-dashed rounded min-w-90px py-3 px-4 mx-2 mb-3">
                  <div className="fs-6 fw-bold text-gray-700">Episode</div>
                  <div className={"fw-semibold " +
                    (series.status == Status.Done ? "text-danger" :
                      series.status == Status.OnBreak ? "text-info" :
                        series.status == Status.Ongoing ? "text-success" :
                          "text-info")}>
                    {
                      series.totalEpisode > 0 ?
                        <>{series.totalEpisode} {series.totalEpisode > 1 ? "episodes" : "episode"}</> :
                        <>Unknown</>
                    }
                  </div>
                </div>
              </div>
              <button className={"btn btn-sm btn-flex btn-center " + (isFollowedByCurrentUser ? "btn-success" : "btn-light")} data-kt-indicator={isLoading ? "on" : "off"} data-kt-follow-btn="true" onClick={() => {
                setFollowStateChanged(true);
              }}>
                <i className={"ki-duotone follow fs-3 " + (isFollowedByCurrentUser ? "ki-check" : "ki-plus")}></i>
                <i className="ki-duotone ki-check following fs-3 d-none"></i>
                <span className="indicator-label">
                  {isFollowedByCurrentUser ? "Following" : "Follow"}</span>
                <span className="indicator-progress">
                  Please wait...
                  <span className="spinner-border spinner-border-sm align-middle ms-2"></span>
                </span>
              </button>
            </div>
          </div>
          <ShowLinkModal key={series.id + "_" + showLinksModal + "_" + tempIntValue} showName={series.name} links={series.links} show={showLinksModal} showId={series.id} />
        </div>
      </OverlayTrigger>
    </>
  )
}

export { ShowComponent }
