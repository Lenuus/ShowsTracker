import { lazy, FC, Suspense } from 'react'
import { Route, Routes, Navigate } from 'react-router-dom'
import { MasterLayout } from '../../_metronic/layout/MasterLayout'
import TopBarProgress from 'react-topbar-progress-indicator'
import { getCSSVariableValue } from '../../_metronic/assets/ts/_utils'
import { WithChildren } from '../../_metronic/helpers'
import { MyShowsPage } from '../pages/my-shows/my-shows-page'
import { CategoryShowsPage } from '../pages/shows/get-shows-by-category'
import { PopularShowsPage } from '../pages/shows/get-popular-shows'
import { AuthPage } from '../modules/auth'
import { DashboardWrapper } from '../pages/dashboard/dashboard-page'
import { AdminVotingPage } from '../pages/admin/votings/voting-page'
import { AdminReportPage } from '../pages/admin/report-page'
import { AdminVotingCreatePage } from '../pages/admin/votings/voting-create-page'
import { ShowTastePage } from '../pages/dashboard/show-taste'
import { RandomShowsPage } from '../pages/shows/get-random-shows'

const PrivateRoutes = () => {

  return (
    <Routes>
      <Route element={<MasterLayout />}>
        {/* Redirect to Dashboard after success login/registartion */}
        <Route path='auth/*' element={<AuthPage />} />
        {/* Pages */}
        <Route path='dashboard' element={<DashboardWrapper />} />
        <Route path='my-shows' element={<MyShowsPage />} />
        <Route path='show-taste' element={<ShowTastePage />} />
        <Route path='find-your-taste' element={<RandomShowsPage />} />
        <Route path='shows'>
          <Route path=':categoryId' element={<CategoryShowsPage />} />
          <Route path='popular' element={<PopularShowsPage />} />
        </Route>
        <Route path='admin'>
          <Route path='votings'>
            <Route path='' element={<AdminVotingPage />} />
            <Route path='create' element={<AdminVotingCreatePage />} />
          </Route>
          <Route path='report' element={<AdminReportPage />} />
        </Route>
        {/* Page Not Found */}
        <Route path='*' element={<Navigate to='/error/404' />} />
      </Route>
    </Routes>
  )
}

const SuspensedView: FC<WithChildren> = ({ children }) => {
  const baseColor = getCSSVariableValue('--bs-primary')
  TopBarProgress.config({
    barColors: {
      '0': baseColor,
    },
    barThickness: 1,
    shadowBlur: 5,
  })
  return <Suspense fallback={<TopBarProgress />}>{children}</Suspense>
}

export { PrivateRoutes }
