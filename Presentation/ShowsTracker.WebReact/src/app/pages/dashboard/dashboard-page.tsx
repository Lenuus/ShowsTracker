import { useIntl } from 'react-intl'
import { PageTitle } from '../../../_metronic/layout/core'
import { useAuth } from '../../modules/auth'
import { Category } from '../../../models/enums/category-enum'
import { WithChildren } from '../../../_metronic/helpers'
import { FC, useEffect, useState } from 'react'
import { ShowComponent } from '../../modules/show/show-component'
import { ShowListModel } from '../../../models/show/show-list-model'
import { get } from '../../api/make-api-request-authorized'
import ServiceResponse from '../../../models/service-response'

const DashboardPage = () => (
  <>
    <OngoingShowsComponent />
    <RandomShowsComponent category={Category.Anime} key={"randomShows_" + Category.Anime} />
    <RandomShowsComponent category={Category.Manga} key={"randomShows_" + Category.Manga} />
    <RandomShowsComponent category={Category.Manhwa} key={"randomShows_" + Category.Manhwa} />
    <RandomShowsComponent category={Category.Manhua} key={"randomShows_" + Category.Manhua} />
    <RandomShowsComponent category={Category.Novel} key={"randomShows_" + Category.Novel} />
  </>
)


const OngoingShowsComponent: FC<WithChildren> = () => {
  const [isLoading, setIsLoading] = useState(false);
  const [randomShows, setRandomShows] = useState<ShowListModel[]>([]);

  useEffect(() => {
    get<ServiceResponse<ShowListModel[]>>('/show/get-ongoing-shows').then((resolve) => {
      if (resolve.data.isSuccesfull) {
        setRandomShows(resolve.data.data);
      }
    })
  }, []);

  const renderShows = () => {
    const bullets = [];
    const totalPageCount = Math.ceil(randomShows.length / 4);
    for (let index = 0; index < totalPageCount; index++) {
      const shows = randomShows.slice(index * 4, (index * 4) + 4);
      bullets.push(
        <>
          <div className={"carousel-item" + (index == 0 ? " active" : "")}>
            <div className='row'>
              {
                shows.map((show) => {
                  return (<ShowComponent key={show.id} series={show} />);
                })
              }
            </div>
          </div>
        </>
      )
    }
    return (bullets);
  }

  return (
    <>
      <div id={"ongoingShows"} className={"carousel carousel-custom slide mb-15 overlay" + (isLoading && " overlay-block")} data-bs-ride="carousel" data-bs-interval="8000">
        <div className="d-flex align-items-center justify-content-between flex-wrap">
          <span className="fs-4 fw-bold pe-2">Ongoing Animes</span>
          <div>
            <a href='#' className="fs-4 fw-bold pe-2" data-bs-target="#ongoingShows" data-bs-slide="prev"><i style={{ color: "#fff" }} className='fs-1 ki-duotone ki-left'></i></a>
            <a href='#' className="fs-4 fw-bold pe-2" data-bs-target="#ongoingShows" data-bs-slide="next"><i style={{ color: "#fff" }} className='fs-1 ki-duotone ki-right'></i></a>
          </div>
        </div>

        <div className="carousel-inner pt-8">
          {renderShows()}
        </div>
      </div>
    </>
  )
}

type RandomShowsProps = {
  category: Category
}
const RandomShowsComponent: FC<RandomShowsProps & WithChildren> = ({ category }) => {
  const [isLoading, setIsLoading] = useState(false);
  const [randomShows, setRandomShows] = useState<ShowListModel[]>([]);

  useEffect(() => {
    get<ServiceResponse<ShowListModel[]>>('/show/get-random-shows?category=' + category).then((resolve) => {
      if (resolve.data.isSuccesfull) {
        setRandomShows(resolve.data.data);
      }
    })
  }, []);

  const renderShows = () => {
    const bullets = [];
    const totalPageCount = Math.ceil(randomShows.length / 4);
    for (let index = 0; index < totalPageCount; index++) {
      const shows = randomShows.slice(index * 4, (index * 4) + 4);
      bullets.push(
        <>
          <div className={"carousel-item" + (index == 0 ? " active" : "")}>
            <div className='row'>
              {
                shows.map((show) => {
                  return (<ShowComponent key={show.id} series={show} />);
                })
              }
            </div>
          </div>
        </>
      )
    }
    return (bullets);
  }

  return (
    <>
      <div id={"randomShows_" + category} className={"carousel carousel-custom slide mb-15 overlay" + (isLoading && " overlay-block")} data-bs-ride="carousel" data-bs-interval="8000">
        <div className="d-flex align-items-center justify-content-between flex-wrap">
          <span className="fs-4 fw-bold pe-2">This Week's Selected&nbsp;
            {category == Category.Anime ? "Animes" :
              category == Category.Manga ? "Mangas" :
                category == Category.Manhua ? "Manhuas" :
                  category == Category.Manhwa ? "Manhwas" :
                    category == Category.Novel ? "Novels" : ""}</span>

          <div>
            <a href='#' className="fs-4 fw-bold pe-2" data-bs-target={"#randomShows_" + category} data-bs-slide="prev"><i style={{ color: "#fff" }} className='fs-1 ki-duotone ki-left'></i></a>
            <a href='#' className="fs-4 fw-bold pe-2" data-bs-target={"#randomShows_" + category} data-bs-slide="next"><i style={{ color: "#fff" }} className='fs-1 ki-duotone ki-right'></i></a>
          </div>
        </div>

        <div className="carousel-inner pt-8">
          {renderShows()}
        </div>
      </div>
    </>
  )
}

const DashboardWrapper = () => {
  const intl = useIntl()
  const auth = useAuth();
  return (
    <>
      <DashboardPage />
    </>
  )
}

export { DashboardWrapper }
