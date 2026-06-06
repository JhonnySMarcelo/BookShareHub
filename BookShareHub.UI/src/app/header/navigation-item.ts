export interface NavigationItem {
  label: string;
  route?: string;
  action?: () => void;
}
