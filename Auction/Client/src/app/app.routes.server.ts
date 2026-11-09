import { RenderMode, ServerRoute } from '@angular/ssr';

export const serverRoutes: ServerRoute[] = [
  { path: 'auction/create', renderMode: RenderMode.Prerender },
  { path: '**', renderMode: RenderMode.Server },
];
