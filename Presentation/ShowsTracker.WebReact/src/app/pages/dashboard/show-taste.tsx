import { useIntl } from 'react-intl'
import { LayoutSetup, PageTitle, useLayout } from '../../../_metronic/layout/core'
import { useAuth } from '../../modules/auth'
import { Category } from '../../../models/enums/category-enum'
import { WithChildren } from '../../../_metronic/helpers'
import { FC, useEffect, useState } from 'react'
import { ShowComponent } from '../../modules/show/show-component'
import { ShowListModel } from '../../../models/show/show-list-model'
import { get } from '../../api/make-api-request-authorized'
import ServiceResponse from '../../../models/service-response'
const ShowTastePage = () => {
  return (<>
    <ShowTasteCategory category={Category.Anime} />
    <ShowTasteCategory category={Category.Manga} />
  </>);
}

type RandomShowsProps = {
  category: Category
}
const ShowTasteCategory: FC<RandomShowsProps & WithChildren> = ({ category }) => {
  const [isLoading, setIsLoading] = useState(false);
  const [randomShows, setRandomShows] = useState<ShowListModel[]>([]);

  useEffect(() => {
    get<ServiceResponse<ShowListModel[]>>('/show/get-random-shows?category=' + category).then((resolve) => {
      if (resolve.data.isSuccesfull) {
        setRandomShows(resolve.data.data);
      }
    })
  }, []);

  return (
    <>
      <link href='show-taste/css/base.css' rel="stylesheet" type="text/css" />
      <h2 className="section-title">
        {category == Category.Anime ? "Animes" :
          category == Category.Manga ? "Mangas" :
            category == Category.Manhua ? "Manhuas" :
              category == Category.Manhwa ? "Manhwas" :
                category == Category.Novel ? "Novels" : ""} for your taste</h2>
      <div className="wrap">
        <div data-stack-1 className="wrap__inner">
          <div className="content content--1">
            {randomShows.map((show) => {
              return (
                <>
                  <div className="card">
                    <div className="card__img" style={{ backgroundImage: "url(" + show.coverImageUrl + ")" }}></div>
                    <h3 className="card__title">{show.name}</h3>
                    <p className="card__description">{show.genres.map((showGenre) => {return showGenre.name;}).join(",")}</p>
                  </div>
                </>
              )
            })}
          </div>
        </div>
      </div>
    </>
  )
}

export { ShowTastePage }
