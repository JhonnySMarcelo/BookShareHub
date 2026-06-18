export function getErrorMessage(error: any): string {
  if (error.status === 0) {
    return 'Unable to connect to the server.';
  }

  if (error.status >= 500) {
    return 'Internal server error.';
  }

  return error.error?.message ?? error.error?.title ?? 'Unexpected error.';
}
