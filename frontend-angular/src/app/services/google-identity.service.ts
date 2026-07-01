import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

declare global {
  interface Window {
    google?: GoogleIdentityWindow['google'];
  }
}

interface GoogleIdentityWindow {
  google?: {
    accounts?: {
      id?: {
        initialize: (options: { client_id: string; callback: (response: any) => void; ux_mode?: string }) => void;
        renderButton: (container: HTMLElement, options: Record<string, unknown>) => void;
      };
    };
  };
}

@Injectable({ providedIn: 'root' })
export class GoogleIdentityService {
  private initialized = false;
  private activeCallback: ((response: any) => void) | null = null;

  init(callback: (response: any) => void): boolean {
    if (typeof window === 'undefined') return false;

    const googleApi = window.google;
    if (!googleApi?.accounts?.id) return false;

    this.activeCallback = callback;

    if (!this.initialized) {
      googleApi.accounts.id.initialize({
        client_id: environment.googleClientId,
        callback: (response: any) => this.activeCallback?.(response),
        ux_mode: 'popup'
      });
      this.initialized = true;
    }

    return true;
  }

  renderButton(container: HTMLElement | null, options?: Record<string, unknown>): boolean {
    if (typeof window === 'undefined') return false;

    const googleApi = window.google;
    if (!container || !googleApi?.accounts?.id) return false;

    googleApi.accounts.id.renderButton(container, {
      theme: 'outline',
      size: 'large',
      width: 300,
      ...options
    });

    return true;
  }

  triggerButton(containerSelector: string): boolean {
    if (typeof window === 'undefined') return false;

    const btn = document.querySelector<HTMLElement>(`${containerSelector} div[role=button]`);
    if (!btn) return false;

    btn.click();
    return true;
  }
}