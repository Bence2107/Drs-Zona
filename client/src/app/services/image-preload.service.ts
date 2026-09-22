import {Injectable} from '@angular/core';

@Injectable({providedIn: 'root'})
export class ImagePreloadService {

  preload(urls: (string | null | undefined)[], timeoutMs = 5000): Promise<void> {
    const valid = urls.filter((url): url is string => !!url);

    if (valid.length === 0) {
      return Promise.resolve();
    }

    const all = Promise.allSettled(valid.map(url => this.loadOne(url))).then(() => void 0);
    const timeout = new Promise<void>(resolve => setTimeout(resolve, timeoutMs));

    return Promise.race([all, timeout]);
  }

  private loadOne(url: string): Promise<void> {
    return new Promise<void>(resolve => {
      const img = new Image();

      const done = () => resolve();

      img.onload = () => {
        if (typeof img.decode === 'function') {
          img.decode().then(done, done);
        } else {
          done();
        }
      };
      img.onerror = done;
      img.src = url;
    });
  }
}
