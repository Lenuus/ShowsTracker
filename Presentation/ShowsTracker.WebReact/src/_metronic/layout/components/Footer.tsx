
import { FC } from 'react'
import { useLayout } from '../core'

const Footer: FC = () => {
  const { classes } = useLayout()
  return (
    <div className='footer py-4 d-flex flex-lg-column' id='kt_footer'>
      {/* begin::Container */}
      <div
        className={`${classes.footerContainer} d-flex flex-column flex-md-row align-items-center justify-content-between`}
      >
        {/* begin::Copyright */}
        <div className='text-gray-900 order-2 order-md-1'></div>
        <div className='text-gray-900 order-2 order-md-1'>
          Powered By <a href='https://myanimelist.net/' target='_blank' className='text-gray-800 text-hover-primary'>MyAnimeList</a>
        </div>
        {/* end::Copyright */}
      </div>
      {/* end::Container */}
    </div>
  )
}

export { Footer }
