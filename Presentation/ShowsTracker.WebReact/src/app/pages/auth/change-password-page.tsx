import { useEffect, useState } from "react"
import { PageTitle } from "../../../_metronic/layout/core"
import { useNavigate, useSearchParams } from "react-router-dom";
import ServiceResponse, { ServiceResponseWithoutData } from "../../../models/service-response";
import { post } from "../../api/make-api-request-authorized";
import { toast } from "react-toastify";

const ChangePasswordPage = () => {
    const [password, setPassword] = useState<string>();
    const [confirmPassword, setConfirmPassword] = useState<string>();
    const [codeIsValid, setCodeIsValid] = useState(false);
    const [isLoading, setIsLoading] = useState(false);
    const [disableButton, setDisableButton] = useState(false);
    const [query, setQuery] = useSearchParams();
    const navigate = useNavigate();

    useEffect(() => { checkCodeIsValid() }, [])

    const checkCodeIsValid = () => {
        setIsLoading(true);
        const userId = query.get("userId");
        const code = query.get("code");

        post<ServiceResponseWithoutData>("/account/check-forgot-password-code", {
            userId,
            code
        }).then((resolve) => {
            if (resolve.data.isSuccesfull) {
                setCodeIsValid(true);
            }
            else {
                setCodeIsValid(false);
            }
            setIsLoading(false);
        }).catch((err) => {
            setCodeIsValid(false);
            setIsLoading(false);
        })
    }

    const resetPassword = () => {
        const userId = query.get("userId");
        const code = query.get("code");
        setIsLoading(true);
        post<ServiceResponseWithoutData>("/account/change-password", {
            userId,
            code,
            password,
            passwordConfirmation: confirmPassword
        }).then((resolve) => {
            if (resolve.data.isSuccesfull) {
                toast("Password changed");
                setDisableButton(true);
                setTimeout(() => {
                    navigate("/auth/login");
                }, 1000);
            }
            else {

            }
            setIsLoading(false);
        }).catch((err) => {
            setIsLoading(false);
        })
    }

    return (
        <>
            {
                codeIsValid && !isLoading ?
                    <>
                        <div className='fv-row mb-8'>
                            <div className='mb-1'>
                                <label className='form-label fw-bolder text-gray-900 fs-6'>Password</label>
                                <div className='position-relative mb-3'>
                                    <input
                                        type='password'
                                        placeholder='Password'
                                        autoComplete='off'
                                        value={password}
                                        onChange={(e) => setPassword(e.target.value)}
                                        className='form-control bg-transparent'
                                    />
                                </div>
                            </div>
                            <div className='mb-2'>
                                <label className='form-label fw-bolder text-gray-900 fs-6'>Confirm Password</label>
                                <div className='position-relative mb-3'>
                                    <input
                                        type='password'
                                        placeholder='Confirm Password'
                                        autoComplete='off'
                                        value={confirmPassword}
                                        onChange={(e) => setConfirmPassword(e.target.value)}
                                        className='form-control bg-transparent'
                                    />
                                </div>
                            </div>
                            <div className='text-muted mb-2'>
                                Use 8 or more characters with a mix of letters, numbers & symbols.
                            </div>
                            <div className='mb-1'>
                                <button className="btn btn-success w-100" disabled={disableButton} data-kt-indicator={isLoading ? "on" : "off"} data-kt-follow-btn="true" onClick={resetPassword}>
                                    <span className="indicator-label">
                                        Save</span>
                                    <span className="indicator-progress">
                                        Please wait...
                                        <span className="spinner-border spinner-border-sm align-middle ms-2"></span>
                                    </span>
                                </button>
                            </div>
                        </div>
                    </>
                    :
                    <>
                        <center><h1>Code is not valid</h1></center>
                    </>
            }
            {isLoading &&
                <>
                    <center><h1>Loading</h1></center>
                </>
            }
        </>
    )
}

export { ChangePasswordPage }