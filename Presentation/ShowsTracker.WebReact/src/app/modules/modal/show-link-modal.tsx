import { FC, useEffect, useRef, useState } from "react"
import { WithChildren } from "../../../_metronic/helpers"
import { Button, Modal } from "react-bootstrap"
import { useAuth } from "../auth"
import { Link } from "react-router-dom"
import { Template } from "webpack"
import { ShowLinkModel } from "../../../models/show/show-link-model"
import { deleteRequest, post } from "../../api/make-api-request-authorized"
import ServiceResponse, { ServiceResponseWithoutData } from "../../../models/service-response"
import { AddLinkToShowRequestModel } from "../../../models/my-shows/add-link-to-show-request-model"
import { isUrlValid } from "../../helpers/url-helpers"
import { UpdateLinkModel } from "../../../models/my-shows/update-link-request-model"

type Props = {
    links: ShowLinkModel[],
    showName: string,
    show: boolean,
    showId: string
}
const ShowLinkModal: FC<Props & WithChildren> = ({ showName, links, show, showId }) => {
    const [showComponent, setShowComponent] = useState(show);
    const handleClose = () => setShowComponent(false);
    const auth = useAuth();
    const [newLinkDesc, setNewLinkDesc] = useState("");
    const [newLinkUrl, setNewLinkUrl] = useState("");
    const [newAddedLinks, setNewAddedLinks] = useState<ShowLinkModel[]>([]);
    const [linkError, setLinkError] = useState(false);

    useEffect(() => {
        if (!isUrlValid(newLinkUrl)) {
            setLinkError(true);
        }
        else {
            setLinkError(false);
        }
    }, [newLinkUrl])

    const addLinkToShow = () => {
        const requestData = new AddLinkToShowRequestModel();
        requestData.name = newLinkDesc;
        requestData.link = newLinkUrl;
        requestData.showId = showId;
        post<ServiceResponse<string>>("/user-show/add-link-to-show", requestData).then((resolve) => {
            const newShow = new ShowLinkModel;
            newShow.id = resolve.data.data;
            newShow.isDefault = false;
            newShow.link = newLinkUrl;
            newShow.name = newLinkDesc;
            links.push(newShow)
            setNewLinkDesc("");
            setNewLinkUrl("");
        }).catch((err) => {
            alert(err.response.data.errorMessage);
        })
    }

    const onDelete = (id: string) => {
        links = links.filter(f => f.id != id);
    }

    return (
        <>
            <Modal
                show={showComponent}
                onHide={handleClose}
                size="lg"
                aria-labelledby="contained-modal-title-vcenter">
                <Modal.Header closeButton>
                    <Modal.Title>Suggested Links for <b>{showName}</b></Modal.Title>
                </Modal.Header>
                <Modal.Body style={{ padding: 0 }}>
                    <div className="container" style={{ padding: 0 }}>
                        <>
                            <table className="table table-responsive table-bordered mb-0">
                                <thead>
                                    <tr>
                                        <th>Name</th>
                                        <th>Link</th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {links.map((link) => {
                                        return (<ExistedRow key={link.id} onDelete={onDelete} link={link} />);
                                    })}
                                    {auth.currentUser &&
                                        <>
                                            <tr>
                                                <td>
                                                    <input
                                                        type="text"
                                                        value={newLinkDesc}
                                                        className="form-control"
                                                        placeholder="Link Description"
                                                        onChange={(e) => {
                                                            setNewLinkDesc(e.target.value);
                                                        }} />
                                                </td>
                                                <td>
                                                    <input
                                                        type="text"
                                                        value={newLinkUrl}
                                                        className="form-control"
                                                        placeholder="Link Url"
                                                        style={{ borderColor: (linkError ? "#FF3767" : "#363843") }}
                                                        onChange={(e) => {
                                                            setNewLinkUrl(e.target.value);
                                                        }} />
                                                </td>
                                                <td>
                                                    <button className="btn btn-icon btn-success me-2" disabled={linkError} onClick={() => {
                                                        addLinkToShow();
                                                    }}>
                                                        <i className="ki-outline ki-plus fs-1"></i>
                                                    </button>
                                                </td>
                                            </tr>
                                        </>
                                    }
                                </tbody>
                            </table>
                        </>
                        {!auth.currentUser &&
                            <>
                                <h4 style={{ textAlign: "center" }} className="m-10">
                                    If you wanna add your own link please <Link to={"/auth/login"}>login</Link></h4>
                            </>
                        }
                    </div>
                </Modal.Body>
                <Modal.Footer>

                </Modal.Footer>
            </Modal>
        </>
    )

}

type ExistedRowProps = {
    link: ShowLinkModel,
    onDelete: any
}
const ExistedRow: FC<ExistedRowProps & WithChildren> = ({ link, onDelete }) => {
    const [currentName, setCurrentName] = useState(link.name);
    const [currentLink, setCurrentLink] = useState(link.link);
    const [linkError, setLinkError] = useState(false);
    const [isDeleted, setIsDeleted] = useState(false);
    const [isEditLoading, setIsEditLoading] = useState(false);
    const [isDeleteLoading, setIsDeleteLoading] = useState(false);

    useEffect(() => {
        if (!isUrlValid(currentLink)) {
            setLinkError(true);
        }
        else {
            setLinkError(false);
        }
    }, [currentLink])

    const deleteLink = () => {
        setIsDeleteLoading(true);
        deleteRequest<ServiceResponseWithoutData>("/user-show/delete-link?id=" + link.id).then((resolve) => {
            onDelete(link.id);
            setIsDeleted(true);
            setIsDeleteLoading(false);
        }).catch((err) => {

            setIsDeleteLoading(false);
        });
    }

    const updateLink = () => {
        setIsEditLoading(true);
        var requestData = new UpdateLinkModel();
        requestData.name = currentName;
        requestData.link = currentLink;
        requestData.id = link.id;
        // @ts-ignore
        post<ServiceResponseWithoutData>("/user-show/update-link", requestData).then((resolve) => {
            setIsEditLoading(false);
        }).catch((err) => {
            setIsEditLoading(false);
        });
    }

    return (
        !isDeleted && <>
            <tr>
                <td>
                    <input
                        type="text"
                        value={currentName}
                        onChange={(e) => { setCurrentName(e.target.value); }}
                        className="form-control" />
                </td>
                <td>
                    <input
                        type="text"
                        value={currentLink}
                        onChange={(e) => { setCurrentLink(e.target.value); }}
                        className="form-control"
                        style={{ borderColor: (linkError ? "#FF3767" : "#363843") }} />
                </td>
                <td>
                    <a href={link.link} target="_blank" className="btn btn-icon btn-primary me-2">
                        <i className="ki-outline ki-general-mouse fs-1"></i>
                    </a>
                    {
                        link.isDefault == false &&
                        <>
                            <button className="btn btn-icon btn-warning me-2" data-kt-indicator={isEditLoading ? "on" : "off"} disabled={linkError} data-kt-follow-btn="true" onClick={updateLink}>
                                <span className="indicator-label">
                                    <i className="ki-outline ki-pencil fs-1"></i></span>
                                <span className="indicator-progress">
                                    <span className="spinner-border spinner-border-sm align-middle"></span>
                                </span>
                            </button>
                            <button className="btn btn-icon btn-danger me-2" data-kt-indicator={isDeleteLoading ? "on" : "off"} disabled={linkError} data-kt-follow-btn="true" onClick={deleteLink}>
                                <span className="indicator-label">
                                    <i className="ki-outline ki-trash fs-1"></i></span>
                                <span className="indicator-progress">
                                    <span className="spinner-border spinner-border-sm align-middle"></span>
                                </span>
                            </button>
                        </>
                    }
                </td>
            </tr>
        </>
    )
}
export { ShowLinkModal }