import React, {FC, useEffect, useRef, useState} from 'react'
import {SearchComponent} from '../../../assets/ts/components'
import {KTIcon, toAbsoluteUrl} from '../../../helpers'

const Search: FC = () => {
  const [menuState, setMenuState] = useState<'main' | 'advanced' | 'preferences'>('main')
  const element = useRef<HTMLDivElement | null>(null)
  const wrapperElement = useRef<HTMLDivElement | null>(null)
  const resultsElement = useRef<HTMLDivElement | null>(null)
  const emptyElement = useRef<HTMLDivElement | null>(null)

  const processs = (search: SearchComponent) => {
    setTimeout(function () {
      const number = Math.floor(Math.random() * 6) + 1

      if (number === 3) {
        // Hide results
        resultsElement.current!.classList.add('d-none')
        // Show empty message
        emptyElement.current!.classList.remove('d-none')
      } else {
        // Show results
        resultsElement.current!.classList.remove('d-none')
        // Hide empty message
        emptyElement.current!.classList.add('d-none')
      }

      // Complete search
      search.complete()
    }, 1500)
  }

  const clear = () => {
    // Hide results
    resultsElement.current!.classList.add('d-none')
    // Hide empty message
    emptyElement.current!.classList.add('d-none')
  }

  useEffect(() => {
    // Initialize search handler
    const searchObject = SearchComponent.createInsance('#kt_header_search')

    // Search handler
    searchObject!.on('kt.search.process', processs)

    // Clear handler
    searchObject!.on('kt.search.clear', clear)
  }, [])

  return (
    <>
      <div
        id='kt_header_search'
        className='d-flex align-items-stretch'
        data-kt-search-keypress='true'
        data-kt-search-min-length='2'
        data-kt-search-enter='enter'
        data-kt-search-layout='menu'
        data-kt-menu-trigger='auto'
        data-kt-menu-overflow='false'
        data-kt-menu-permanent='true'
        data-kt-menu-placement='bottom-end'
        ref={element}
      >
        <div
          className='d-flex align-items-center'
          data-kt-search-element='toggle'
          id='kt_header_search_toggle'
        >
          <div className='btn btn-icon btn-active-light-primary btn-custom w-30px h-30px w-md-40px h-md-40px'>
            <KTIcon iconName='magnifier' className='fs-1' />
          </div>
        </div>

        <div
          data-kt-search-element='content'
          className='menu menu-sub menu-sub-dropdown p-7 w-325px w-md-375px'
        >
          <div
            className={`${menuState === 'main' ? '' : 'd-none'}`}
            ref={wrapperElement}
            data-kt-search-element='wrapper'
          >
            <form
              data-kt-search-element='form'
              className='w-100 position-relative mb-3'
              autoComplete='off'
            >
              <KTIcon
                iconName='magnifier'
                className='fs-2 text-lg-1 text-gray-500 position-absolute top-50 translate-middle-y ms-0'
              />

              <input
                type='text'
                className='form-control form-control-flush ps-10'
                name='search'
                placeholder='Search...'
                data-kt-search-element='input'
              />

              <span
                className='position-absolute top-50 end-0 translate-middle-y lh-0 d-none me-1'
                data-kt-search-element='spinner'
              >
                <span className='spinner-border h-15px w-15px align-middle text-gray-500' />
              </span>

              <span
                className='btn btn-flush btn-active-color-primary position-absolute top-50 end-0 translate-middle-y lh-0 d-none'
                data-kt-search-element='clear'
              >
                <KTIcon iconName='cross' className='fs-2 text-lg-1 me-0' />
              </span>

              <div
                className='position-absolute top-50 end-0 translate-middle-y'
                data-kt-search-element='toolbar'
              >
                <div
                  data-kt-search-element='preferences-show'
                  className='btn btn-icon w-20px btn-sm btn-active-color-primary me-1'
                  data-bs-toggle='tooltip'
                  onClick={() => {
                    setMenuState('preferences')
                  }}
                  title='Show search preferences'
                >
                  <KTIcon iconName='setting-2' className='fs-1' />
                </div>

                <div
                  data-kt-search-element='advanced-options-form-show'
                  className='btn btn-icon w-20px btn-sm btn-active-color-primary'
                  data-bs-toggle='tooltip'
                  onClick={() => {
                    setMenuState('advanced')
                  }}
                  title='Show more search options'
                >
                  <KTIcon iconName='down' className='fs-2' />
                </div>
              </div>
            </form>

            <div ref={resultsElement} data-kt-search-element='results' className='d-none'>
              <div className='scroll-y mh-200px mh-lg-350px'>
                <h3 className='fs-5 text-muted m-0 pb-5' data-kt-search-element='category-title'>
                  Users
                </h3>

                <a
                  href='/#'
                  className='d-flex text-gray-900 text-hover-primary align-items-center mb-5'
                >
                  <div className='symbol symbol-40px me-4'>
                    <img src={toAbsoluteUrl('media/avatars/300-6.jpg')} alt='' />
                  </div>

                  <div className='d-flex flex-column justify-content-start fw-semibold'>
                    <span className='fs-6 fw-semibold'>Karina Clark</span>
                    <span className='fs-7 fw-semibold text-muted'>Marketing Manager</span>
                  </div>
                </a>
              </div>
            </div>

            <div ref={emptyElement} data-kt-search-element='empty' className='text-center d-none'>
              <div className='pt-10 pb-10'>
                <KTIcon iconName='search-list' className='fs-4x opacity-50' />
              </div>

              <div className='pb-15 fw-semibold'>
                <h3 className='text-gray-600 fs-5 mb-2'>No result found</h3>
                <div className='text-muted fs-7'>Please try again with a different query</div>
              </div>
            </div>
          </div>

          <form className={`pt-1 ${menuState === 'advanced' ? '' : 'd-none'}`}>
            <h3 className='fw-semibold text-gray-900 mb-7'>Advanced Search</h3>

            <div className='mb-5'>
              <input
                type='text'
                className='form-control form-control-sm form-control-solid'
                placeholder='Contains the word'
                name='query'
              />
            </div>

            <div className='mb-5'>
              <div className='nav-group nav-group-fluid'>
                <label>
                  <input
                    type='radio'
                    className='btn-check'
                    name='type'
                    value='has'
                    defaultChecked
                  />
                  <span className='btn btn-sm btn-color-muted btn-active btn-active-primary'>
                    All
                  </span>
                </label>

                <label>
                  <input type='radio' className='btn-check' name='type' value='users' />
                  <span className='btn btn-sm btn-color-muted btn-active btn-active-primary px-4'>
                    Users
                  </span>
                </label>

                <label>
                  <input type='radio' className='btn-check' name='type' value='orders' />
                  <span className='btn btn-sm btn-color-muted btn-active btn-active-primary px-4'>
                    Orders
                  </span>
                </label>

                <label>
                  <input type='radio' className='btn-check' name='type' value='projects' />
                  <span className='btn btn-sm btn-color-muted btn-active btn-active-primary px-4'>
                    Projects
                  </span>
                </label>
              </div>
            </div>

            <div className='mb-5'>
              <input
                type='text'
                name='assignedto'
                className='form-control form-control-sm form-control-solid'
                placeholder='Assigned to'
              />
            </div>

            <div className='mb-5'>
              <input
                type='text'
                name='collaborators'
                className='form-control form-control-sm form-control-solid'
                placeholder='Collaborators'
              />
            </div>

            <div className='mb-5'>
              <div className='nav-group nav-group-fluid'>
                <label>
                  <input
                    type='radio'
                    className='btn-check'
                    name='attachment'
                    value='has'
                    defaultChecked
                  />
                  <span className='btn btn-sm btn-color-muted btn-active btn-active-primary'>
                    Has attachment
                  </span>
                </label>

                <label>
                  <input type='radio' className='btn-check' name='attachment' value='any' />
                  <span className='btn btn-sm btn-color-muted btn-active btn-active-primary px-4'>
                    Any
                  </span>
                </label>
              </div>
            </div>

            <div className='mb-5'>
              <select
                name='timezone'
                aria-label='Select a Timezone'
                data-control='select2'
                data-placeholder='date_period'
                className='form-select form-select-sm form-select-solid'
              >
                <option value='next'>Within the next</option>
                <option value='last'>Within the last</option>
                <option value='between'>Between</option>
                <option value='on'>On</option>
              </select>
            </div>

            <div className='row mb-8'>
              <div className='col-6'>
                <input
                  type='number'
                  name='date_number'
                  className='form-control form-control-sm form-control-solid'
                  placeholder='Lenght'
                />
              </div>

              <div className='col-6'>
                <select
                  name='date_typer'
                  aria-label='Select a Timezone'
                  data-control='select2'
                  data-placeholder='Period'
                  className='form-select form-select-sm form-select-solid'
                >
                  <option value='days'>Days</option>
                  <option value='weeks'>Weeks</option>
                  <option value='months'>Months</option>
                  <option value='years'>Years</option>
                </select>
              </div>
            </div>

            <div className='d-flex justify-content-end'>
              <button
                onClick={(e) => {
                  e.preventDefault()
                  setMenuState('main')
                }}
                className='btn btn-sm btn-light fw-bold btn-active-light-primary me-2'
              >
                Cancel
              </button>

              <a
                href='/#'
                className='btn btn-sm fw-bold btn-primary'
                data-kt-search-element='advanced-options-form-search'
              >
                Search
              </a>
            </div>
          </form>

          <form className={`pt-1 ${menuState === 'preferences' ? '' : 'd-none'}`}>
            <h3 className='fw-semibold text-gray-900 mb-7'>Search Preferences</h3>

            <div className='pb-4 border-bottom'>
              <label className='form-check form-switch form-switch-sm form-check-custom form-check-solid flex-stack'>
                <span className='form-check-label text-gray-700 fs-6 fw-semibold ms-0 me-2'>
                  Projects
                </span>

                <input className='form-check-input' type='checkbox' value='1' defaultChecked />
              </label>
            </div>

            <div className='py-4 border-bottom'>
              <label className='form-check form-switch form-switch-sm form-check-custom form-check-solid flex-stack'>
                <span className='form-check-label text-gray-700 fs-6 fw-semibold ms-0 me-2'>
                  Targets
                </span>
                <input className='form-check-input' type='checkbox' value='1' defaultChecked />
              </label>
            </div>

            <div className='py-4 border-bottom'>
              <label className='form-check form-switch form-switch-sm form-check-custom form-check-solid flex-stack'>
                <span className='form-check-label text-gray-700 fs-6 fw-semibold ms-0 me-2'>
                  Affiliate Programs
                </span>
                <input className='form-check-input' type='checkbox' value='1' />
              </label>
            </div>

            <div className='py-4 border-bottom'>
              <label className='form-check form-switch form-switch-sm form-check-custom form-check-solid flex-stack'>
                <span className='form-check-label text-gray-700 fs-6 fw-semibold ms-0 me-2'>
                  Referrals
                </span>
                <input className='form-check-input' type='checkbox' value='1' defaultChecked />
              </label>
            </div>

            <div className='py-4 border-bottom'>
              <label className='form-check form-switch form-switch-sm form-check-custom form-check-solid flex-stack'>
                <span className='form-check-label text-gray-700 fs-6 fw-semibold ms-0 me-2'>
                  Users
                </span>
                <input className='form-check-input' type='checkbox' />
              </label>
            </div>

            <div className='d-flex justify-content-end pt-7'>
              <button
                onClick={(e) => {
                  e.preventDefault()
                  setMenuState('main')
                }}
                className='btn btn-sm btn-light fw-bold btn-active-light-primary me-2'
              >
                Cancel
              </button>
              <button className='btn btn-sm fw-bold btn-primary'>Save Changes</button>
            </div>
          </form>
        </div>
      </div>
    </>
  )
}

export {Search}
