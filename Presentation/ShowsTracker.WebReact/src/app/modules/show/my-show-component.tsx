import { FC, RefAttributes, useEffect, useState } from "react"
import { WithChildren } from "../../../_metronic/helpers"
import { Status } from "../../../models/enums/status-enum"
import { Category } from "../../../models/enums/category-enum"
import { ReleaseType } from "../../../models/enums/release-type-enum"
import { useAuth } from "../auth"
import { useNavigate } from "react-router-dom"
import { post } from "../../api/make-api-request-authorized"
import ServiceResponse from "../../../models/service-response"
import { MyShowListModel } from "../../../models/my-shows/my-show-list-response-model"
import moment from "moment"
import { resolve } from "path"
import { date } from "yup"
import { TrackStatus } from "../../../models/enums/track-status-enum"
import { ShowLinkModal } from "../modal/show-link-modal"
import { toast } from "react-toastify"
import { OverlayTrigger, Tooltip } from "react-bootstrap"

type Props = {
  series: MyShowListModel,
  setRefreshData: any
}
const MyShowComponent: FC<Props & WithChildren> = ({ series, setRefreshData }) => {
  const { currentUser, logout } = useAuth()
  const [currentEpisode, setCurrentEpisode] = useState(series.currentEpisode);
  const [lastUpdateDate, setLastUpdateDate] = useState<any>(series.lastUpdateDate);
  const [updatedCurrentEpisode, setUpdatedCurrentEpisode] = useState(0);
  const [showUpdateButton, setShowUpdateButton] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [trackStatus, setTrackStatus] = useState<TrackStatus>(series.trackStatus);
  const [showLinksModal, setShowLinksModal] = useState(false);
  const [tempIntValue, setTempIntValue] = useState(0);
  const navigate = useNavigate();
  const isNumeric = (str: string) => {
    return !isNaN(parseFloat(str))
  }

  const unfollowShow = () => {
    setIsLoading(true);
    post<ServiceResponse<boolean>>("/user-show/drop-show", { showId: series.showId }).then((resolve) => {
      if (resolve.data.isSuccesfull) {
        toast("Updated")
        setRefreshData(true);
      }
      setIsLoading(false);
    }).catch((resolve) => {
      setIsLoading(false);
    });
  }

  const updateCurrentEpisode = () => {
    setIsLoading(true);
    post<ServiceResponse<boolean>>("/user-show/update-show", {
      id: series.id,
      currentEpisode: updatedCurrentEpisode
    }).then((resolve) => {
      if (resolve.data.isSuccesfull) {
        setShowUpdateButton(false);
        setLastUpdateDate(moment.utc());
        setCurrentEpisode(updatedCurrentEpisode);
      }
      setIsLoading(false);
      toast("Updated")
    }).catch((resolve) => {
      setIsLoading(false);
    })
  }

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
        <div className="col-md-4 col-xs-6 col-xl-3 col-xxl-3 mb-5" data-bs-toggle="tooltip" data-bs-html="true" title="<em>Tooltip</em> <u>with</u> <b>HTML</b>">
          <div className="card">
            <div className="ribbon ribbon-triangle ribbon-top-end border-dark" onClick={() => {
              setShowLinksModal(true);
              setTempIntValue(tempIntValue + 1);
            }} style={{ zIndex: "0" }}>
              <div className="ribbon-icon mt-n5 me-n6">
                <i className="bi bi-three-dots fs-2 text-white"></i>
              </div>
            </div>
            <div className="ribbon ribbon-triangle ribbon-top-start border-dark" onClick={() => {
              unfollowShow();
            }} style={{ zIndex: "0" }}>
              <div className="ribbon-icon mt-n5 ms-n5">
                <i className="las la-heart-broken fs-2 text-white"></i>
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
                    margin: "auto"
                  }} />
                </div>
              </div>

              <span className="fs-4 text-gray-800 fw-bold mb-0">{series.name.length > 25 ? series.name.substring(0, 25) + "..." : series.name}</span>
              <div className="fw-semibold text-gray-500 mb-6">
                {series.category == Category.Anime ? "Anime" :
                  series.category == Category.Manga ? "Manga" :
                    series.category == Category.Manhua ? "Manhua" :
                      series.category == Category.Manhwa ? "Manhwa" :
                        series.category == Category.Novel ? "Novel" :
                          ""}
              </div>
              <div className="d-flex flex-center flex-wrap mb-5">
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
                  <div className="fs-6 fw-bold text-gray-700">Track Status</div>
                  <div className={"fw-semibold " +
                    (trackStatus == TrackStatus.Done ? "text-danger" :
                      trackStatus == TrackStatus.OnBreak ? "text-info" :
                        trackStatus == TrackStatus.Dropped ? "text-danger" :
                          trackStatus == TrackStatus.Ongoing ? "text-success" :
                            "text-info")}>
                    {trackStatus == TrackStatus.Done ? "Done" :
                      trackStatus == TrackStatus.OnBreak ? "Onbreak" :
                        trackStatus == TrackStatus.Dropped ? "Dropped" :
                          trackStatus == TrackStatus.Ongoing ? "Ongoing" :
                            "Unknown"}
                  </div>
                </div>

                <div className="border border-dashed rounded min-w-90px py-3 px-4 mx-2 mb-3">
                  <div className="fs-6 fw-bold text-gray-700">Last Update</div>
                  <div className={"fw-semibold " +
                    (series.status == Status.Done ? "text-danger" :
                      series.status == Status.OnBreak ? "text-info" :
                        series.status == Status.Ongoing ? "text-success" :
                          "text-info")}>
                    {moment(lastUpdateDate).format("LL")}
                  </div>
                </div>
              </div>
              <div className="border border-dashed rounded min-w-90px py-3 px-4 mx-2 mb-3">
                <div className="fs-6 fw-bold text-gray-700 text-center">Current Episode</div>
                <div className="input-group w-md-150px">
                  <input type="text" className="form-control text-center text-success" placeholder="Amount" defaultValue={currentEpisode} onChange={(e) => {
                    var val = e.target.value;
                    var newVal = 0;
                    if (isNumeric(val)) {
                      newVal = parseInt(val);
                      if (newVal != currentEpisode && newVal > 0 && (series.totalEpisode <= 0 || (series.totalEpisode > 0 && series.totalEpisode >= newVal))) {
                        setShowUpdateButton(true);
                        setUpdatedCurrentEpisode(newVal);
                      }
                      else {
                        setShowUpdateButton(false);
                      }
                    }
                  }} />
                  {
                    showUpdateButton ?

                      <button className={"btn btn-icon btn-outline btn-active-color-success " + (showUpdateButton ? "" : "d-none")} data-kt-indicator={isLoading ? "on" : "off"} data-kt-follow-btn="true" onClick={() => {
                        updateCurrentEpisode();
                        if (updatedCurrentEpisode == series.totalEpisode) {
                          setTrackStatus(TrackStatus.Done);
                        }
                        else {
                          setTrackStatus(TrackStatus.Ongoing);
                        }
                      }}>
                        <span className="indicator-label">
                          <i className="ki-duotone ki-check fs-2"></i></span>
                        <span className="indicator-progress"><span className="spinner-border spinner-border-sm align-middle"></span>
                        </span>
                      </button>
                      : <></>
                  }
                </div>
              </div>
            </div>
          </div>
        </div>
      </OverlayTrigger>
      <ShowLinkModal key={series.id + "_" + showLinksModal + "_" + tempIntValue} showName={series.name} links={series.links} show={showLinksModal} showId={series.showId} />
    </>
  )
}

export { MyShowComponent }
