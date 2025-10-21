const API_BASE_URL = 'http://localhost:5000/api';

export interface Disc {
    id: number;
    name: string;
    category: DiscCategory;
    brand: string;
    description: string;
    speed: number; // Can handle decimals in JavaScript
    glide: number; // Can handle decimals in JavaScript  
    turn: number;  // Can handle decimals in JavaScript
    fade: number;  // Can handle decimals in JavaScript
    plastic: string;
    color: string;
    weight: number;
    imagePath?: string;
    imagePositionX?: number;
    imagePositionY?: number;
    imageZoom?: number;
    createdDate: string;
    flightNumbers: string;
    // Local properties (not stored in API)
    inTheBag?: boolean;
    forSale?: boolean;
    isFavorite?: boolean;
}

export enum DiscCategory {
    Putter = 1,
    Mid = 2,
    Fairway = 3,
    Driver = 4
}

export interface CategoryInfo {
    id: number;
    name: string;
}

class ApiService {
    private async request<T>(endpoint: string, options?: RequestInit): Promise<T> {
        const response = await fetch(`${API_BASE_URL}${endpoint}`, {
            headers: {
                'Content-Type': 'application/json',
                ...options?.headers,
            },
            ...options,
        });

        if (!response.ok) {
            throw new Error(`API Error: ${response.status} ${response.statusText}`);
        }

        // Handle empty responses (like DELETE operations)
        const contentType = response.headers.get('content-type');
        if (!contentType || !contentType.includes('application/json')) {
            return null as T;
        }

        const text = await response.text();
        if (!text || text.trim() === '') {
            return null as T;
        }

        return JSON.parse(text);
    }

    async getAllDiscs(): Promise<Disc[]> {
        return this.request<Disc[]>('/discs');
    }

    async getDisc(id: number): Promise<Disc> {
        return this.request<Disc>(`/discs/${id}`);
    }

    async getDiscsByCategory(category: DiscCategory): Promise<Disc[]> {
        return this.request<Disc[]>(`/discs/category/${category}`);
    }

    async createDisc(disc: Omit<Disc, 'id' | 'createdDate' | 'flightNumbers'>): Promise<Disc> {
        return this.request<Disc>('/discs', {
            method: 'POST',
            body: JSON.stringify(disc),
        });
    }

    async updateDisc(id: number, disc: Disc): Promise<void> {
        await this.request<void>(`/discs/${id}`, {
            method: 'PUT',
            body: JSON.stringify(disc),
        });
    }

    async updateImagePosition(id: number, positionX: number, positionY: number, zoom: number): Promise<void> {
        await this.request<void>(`/discs/${id}/image-position`, {
            method: 'PATCH',
            body: JSON.stringify({
                imagePositionX: positionX,
                imagePositionY: positionY,
                imageZoom: zoom
            }),
        });
    }

    async toggleFavorite(id: number, isFavorite: boolean): Promise<void> {
        await this.request<void>(`/discs/${id}/favorite`, {
            method: 'PATCH',
            body: JSON.stringify({ isFavorite }),
        });
    }

    async deleteDisc(id: number): Promise<void> {
        await this.request<void>(`/discs/${id}`, {
            method: 'DELETE',
        });
    }

    async uploadDiscImage(id: number, file: File): Promise<{ imagePath: string }> {
        const formData = new FormData();
        formData.append('image', file);

        const response = await fetch(`${API_BASE_URL}/discs/${id}/image`, {
            method: 'POST',
            body: formData,
        });

        if (!response.ok) {
            throw new Error(`Image upload failed: ${response.status} ${response.statusText}`);
        }

        return response.json();
    }

    async getCategories(): Promise<CategoryInfo[]> {
        return this.request<CategoryInfo[]>('/discs/categories');
    }

    getCategoryName(category: DiscCategory): string {
        switch (category) {
            case DiscCategory.Putter: return 'Putter';
            case DiscCategory.Mid: return 'Mid';
            case DiscCategory.Fairway: return 'Fairway';
            case DiscCategory.Driver: return 'Driver';
            default: return 'Unknown';
        }
    }

    getImageUrl(imagePath?: string): string {
        if (!imagePath) return '/placeholder-disc.png';
        return `http://localhost:5000${imagePath}`;
    }
}

export const apiService = new ApiService();
