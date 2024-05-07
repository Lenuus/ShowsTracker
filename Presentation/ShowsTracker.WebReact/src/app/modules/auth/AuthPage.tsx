import { Route, Routes } from 'react-router-dom'
import { Registration } from './components/Registration'
import { ForgotPassword } from './components/ForgotPassword'
import { Login } from './components/Login'
import { AuthLayout } from './AuthLayout'
import { ChangePasswordPage } from '../../pages/auth/change-password-page'
import { ConfirmEmailPage } from '../../pages/auth/confirm-email-page'

const AuthPage = () => (
  <Routes>
    <Route element={<AuthLayout />}>
      <Route path='login' element={<Login />} />
      <Route path='registration' element={<Registration />} />
      <Route path='forgot-password' element={<ForgotPassword />} />
      <Route path='change-password' element={<ChangePasswordPage />} />
      <Route path='confirm-email' element={<ConfirmEmailPage />} />
      <Route index element={<Login />} />
    </Route>
  </Routes>
)

export { AuthPage }
