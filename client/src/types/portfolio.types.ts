export interface PortfolioPosition {
  symbol: string;
  company: string;
  quantity: number;
  avgPrice: number;
  marketValue: number;
  dayPL: number;
  dayPLPercent: number;
  color: string;
}

export interface TableSortConfig {
  field: keyof PortfolioPosition | null;
  direction: 'asc' | 'desc';
}

export interface TableFilters {
  searchText: string;
  showFilters: boolean;
}