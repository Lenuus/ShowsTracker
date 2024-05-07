import { FC, useEffect, useState } from 'react'
import { Outlet, useLocation, useNavigate } from 'react-router-dom'
import { Footer } from './components/Footer'
import { HeaderWrapper } from './components/header/HeaderWrapper'
import { Toolbar } from './components/toolbar/Toolbar'
import { ScrollTop } from './components/ScrollTop'
import { Content } from './components/Content'
import { PageDataProvider, useLayout } from './core'
import { MenuComponent } from '../assets/ts/components'
import clsx from 'clsx'
import { WithChildren } from '../helpers'
import { ToastContainer } from 'react-toastify'
import 'react-toastify/dist/ReactToastify.css';

const MasterLayout: FC<WithChildren> = ({ children }) => {
  const { classes, noLayout } = useLayout()
  const location = useLocation()

  useEffect(() => {
    setTimeout(() => {
      MenuComponent.reinitialization()
    }, 500)
  }, [location.key])

  return (<>
    {
      noLayout ?
        <>
          <Outlet />
        </>
        :
        <>
          <PageDataProvider>
            <div className='page d-flex flex-row flex-column-fluid'>
              <div className='wrapper d-flex flex-column flex-row-fluid' id='kt_wrapper'>
                <HeaderWrapper />

                <div id='kt_content' className='content d-flex flex-column flex-column-fluid'>
                  <Toolbar />
                  <div
                    className={clsx(
                      'd-flex flex-column-fluid align-items-start',
                      classes.contentContainer.join(' ')
                    )}
                    id='kt_post'
                  >
                    <Content>
                      <Outlet />
                    </Content>
                  </div>
                </div>
                <Footer />
              </div>
            </div>
            <ScrollTop />
            <ToastContainer />
          </PageDataProvider>
        </>
    }

  </>
  )
}

export { MasterLayout }
