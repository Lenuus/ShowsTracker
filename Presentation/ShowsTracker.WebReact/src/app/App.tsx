import { Suspense, useState } from 'react'
import { Outlet } from 'react-router-dom'
import { I18nProvider } from '../_metronic/i18n/i18nProvider'
import { LayoutProvider, LayoutSplashScreen } from '../_metronic/layout/core'
import { MasterInit } from '../_metronic/layout/MasterInit'
import { AuthInit } from './modules/auth'
import BasicToast from './modules/common/toast-message'
import { ToastContainer, toast } from 'react-toastify';
import { GoogleOAuthProvider } from '@react-oauth/google';

const App = () => {
  const [errorMessage, setErrorMessage] = useState("");
  const [showErrorMessage, setShowErrorMessage] = useState(false);
  return (
    <GoogleOAuthProvider clientId="660699910730-soi6gs4d7lt578imq4t8s8g94skg0684.apps.googleusercontent.com">
      <Suspense fallback={<LayoutSplashScreen />}>
        <I18nProvider>
          <LayoutProvider>
            <AuthInit>
              <Outlet />
              <MasterInit />
            </AuthInit>
          </LayoutProvider>
        </I18nProvider>
      </Suspense>
    </GoogleOAuthProvider>
  )
}

export { App }
