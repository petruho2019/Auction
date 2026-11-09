# Auction Client

Angular client for creating auctions. The application uses server-side rendering and prerenders `/auction/create`.

## Run locally

```bash
pnpm install
pnpm start
```

The form submits `multipart/form-data` to the Auction API at `http://localhost:5077/api/v1/auctions`. Start the API with its HTTP launch profile before submitting the form.

## Build and serve SSR

```bash
pnpm build
pnpm run serve:ssr:auction-client
```

## Run tests

```bash
pnpm exec ng test --watch=false
```
