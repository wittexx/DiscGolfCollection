import { writable, type Writable } from 'svelte/store';
import { browser } from '$app/environment';

// Theme store
export type Theme = 'light' | 'dark';
export const theme: Writable<Theme> = writable('light');

// Local storage utilities
class LocalStorageManager {
    private getItem<T>(key: string, defaultValue: T): T {
        if (!browser) return defaultValue;
        
        try {
            const item = localStorage.getItem(key);
            return item ? JSON.parse(item) : defaultValue;
        } catch (error) {
            console.error(`Error reading from localStorage for key "${key}":`, error);
            return defaultValue;
        }
    }

    private setItem<T>(key: string, value: T): void {
        if (!browser) return;
        
        try {
            localStorage.setItem(key, JSON.stringify(value));
        } catch (error) {
            console.error(`Error writing to localStorage for key "${key}":`, error);
        }
    }

    // Theme management
    getTheme(): Theme {
        return this.getItem('disc-golf-theme', 'light');
    }

    setTheme(newTheme: Theme): void {
        this.setItem('disc-golf-theme', newTheme);
        theme.set(newTheme);
    }

    // Bag management (stores array of disc IDs)
    getBagDiscIds(): number[] {
        return this.getItem('disc-golf-bag', []);
    }

    setBagDiscIds(discIds: number[]): void {
        this.setItem('disc-golf-bag', discIds);
    }

    addToBag(discId: number): void {
        const bagDiscIds = this.getBagDiscIds();
        if (!bagDiscIds.includes(discId)) {
            bagDiscIds.push(discId);
            this.setBagDiscIds(bagDiscIds);
        }
    }

    removeFromBag(discId: number): void {
        const bagDiscIds = this.getBagDiscIds();
        const updatedBag = bagDiscIds.filter(id => id !== discId);
        this.setBagDiscIds(updatedBag);
    }

    isInBag(discId: number): boolean {
        return this.getBagDiscIds().includes(discId);
    }

    clearBag(): void {
        this.setBagDiscIds([]);
    }

    getBagCount(): number {
        return this.getBagDiscIds().length;
    }

    // For Sale management (stores array of disc IDs)
    getForSaleDiscIds(): number[] {
        return this.getItem('disc-golf-for-sale', []);
    }

    setForSaleDiscIds(discIds: number[]): void {
        this.setItem('disc-golf-for-sale', discIds);
    }

    addToForSale(discId: number): void {
        const forSaleDiscIds = this.getForSaleDiscIds();
        if (!forSaleDiscIds.includes(discId)) {
            forSaleDiscIds.push(discId);
            this.setForSaleDiscIds(forSaleDiscIds);
        }
    }

    removeFromForSale(discId: number): void {
        const forSaleDiscIds = this.getForSaleDiscIds();
        const updatedForSale = forSaleDiscIds.filter(id => id !== discId);
        this.setForSaleDiscIds(updatedForSale);
    }

    isForSale(discId: number): boolean {
        return this.getForSaleDiscIds().includes(discId);
    }

    clearForSale(): void {
        this.setForSaleDiscIds([]);
    }

    getForSaleCount(): number {
        return this.getForSaleDiscIds().length;
    }

    // Favorites management (stores array of disc IDs)
    getFavoriteDiscIds(): number[] {
        return this.getItem('disc-golf-favorites', []);
    }

    setFavoriteDiscIds(discIds: number[]): void {
        this.setItem('disc-golf-favorites', discIds);
    }

    addToFavorites(discId: number): void {
        const favoriteDiscIds = this.getFavoriteDiscIds();
        if (!favoriteDiscIds.includes(discId)) {
            favoriteDiscIds.push(discId);
            this.setFavoriteDiscIds(favoriteDiscIds);
        }
    }

    removeFromFavorites(discId: number): void {
        const favoriteDiscIds = this.getFavoriteDiscIds();
        const updatedFavorites = favoriteDiscIds.filter(id => id !== discId);
        this.setFavoriteDiscIds(updatedFavorites);
    }

    isFavorite(discId: number): boolean {
        return this.getFavoriteDiscIds().includes(discId);
    }

    clearFavorites(): void {
        this.setFavoriteDiscIds([]);
    }

    getFavoriteCount(): number {
        return this.getFavoriteDiscIds().length;
    }
}

export const localStorageManager = new LocalStorageManager();

// Initialize theme from localStorage on app start
if (browser) {
    const savedTheme = localStorageManager.getTheme();
    theme.set(savedTheme);
    
    // Apply theme to document
    document.documentElement.setAttribute('data-theme', savedTheme);
    
    // Listen for theme changes and apply to document
    theme.subscribe(newTheme => {
        document.documentElement.setAttribute('data-theme', newTheme);
    });
}
