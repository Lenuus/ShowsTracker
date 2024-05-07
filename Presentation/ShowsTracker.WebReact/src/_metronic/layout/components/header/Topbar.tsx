
import { FC } from 'react'
import clsx from 'clsx'
import { KTIcon, toAbsoluteUrl } from '../../../helpers'
import { useAuth } from '../../../../app/modules/auth'
import {
  HeaderNotificationsMenu,
  HeaderUserMenu,
  Search
} from '../../../partials'
import { Link } from 'react-router-dom'

const toolbarButtonMarginClass = 'ms-1 ms-lg-3',
  toolbarButtonHeightClass = 'btn-active-light-primary btn-custom w-30px h-30px w-md-40px h-md-40p',
  toolbarUserAvatarHeightClass = 'symbol-30px symbol-md-40px',
  toolbarButtonIconSizeClass = 'fs-1'

const Topbar: FC = () => {
  const { currentUser, logout } = useAuth()
  return (
    <div className='d-flex align-items-stretch flex-shrink-0'>
      <div className='topbar d-flex align-items-stretch flex-shrink-0'>
        {/* begin::User */}
        {currentUser ?
          <>
            <div
              className={clsx('d-flex align-items-center', toolbarButtonMarginClass)}
              id='kt_header_user_menu_toggle'
            >
              {/* begin::Toggle */}
              <div
                className={clsx('cursor-pointer symbol', toolbarUserAvatarHeightClass)}
                data-kt-menu-trigger='click'
                data-kt-menu-attach='parent'
                data-kt-menu-placement='bottom-end'
                data-kt-menu-flip='bottom'
              >
                <img
                  className='h-30px w-30px rounded'
                  src={toAbsoluteUrl('media/avatars/blank.png')}
                  alt='metronic'
                />
              </div>

              <HeaderUserMenu />
              {/* end::Toggle */}
            </div>
          </>
          :
          <>
            {/* begin::Toggle */}
            <Link to="/auth/login" className="btn btn-icon btn-light pulse">
              <i className="ki-duotone ki-user-tick fs-1">
                <span className="path1"></span>
                <span className="path2"></span>
                <span className="path3"></span>
                <span className="path4"></span>
              </i>
              <span className="pulse-ring"></span>
            </Link>
            <div
              className={clsx('d-flex align-items-center', toolbarButtonMarginClass)}
              id='kt_header_user_menu_toggle'
            >
              <Link to='/auth/login'>Login</Link>
            </div>
            {/* end::Toggle */}
          </>}
        {/* end::User */}
      </div>
    </div>
  )
}

export { Topbar }
