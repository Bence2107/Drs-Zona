import { MatPaginatorIntl } from '@angular/material/paginator';

export function getHungarianPaginatorIntl() {
  const paginatorIntl = new MatPaginatorIntl();

  paginatorIntl.itemsPerPageLabel = 'Elemek oldalanként:';
  paginatorIntl.nextPageLabel = 'Következő oldal';
  paginatorIntl.previousPageLabel = 'Előző oldal';
  paginatorIntl.firstPageLabel = 'Első oldal';
  paginatorIntl.lastPageLabel = 'Útolsó oldal';

  paginatorIntl.getRangeLabel = (page: number, pageSize: number, length: number) => {
    if (length === 0 || pageSize === 0) return `0 / 0`;

    const totalPages = Math.ceil(length / pageSize);
    return `${page + 1} / ${totalPages}. oldal`;
  };

  return paginatorIntl;
}
