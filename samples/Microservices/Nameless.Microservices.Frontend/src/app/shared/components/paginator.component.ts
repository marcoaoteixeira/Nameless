import { Component, computed, input, output } from '@angular/core';

@Component({
  selector: 'app-paginator',
  template: `
    @if (totalPages() > 1) {
      <div class="flex items-center justify-between">
        <p class="text-sm text-gray-500">
          Showing {{ rangeStart() }}–{{ rangeEnd() }} of {{ total() }}
        </p>
        <div class="flex gap-1" role="navigation" aria-label="Pagination">
          <button
            [disabled]="currentPage() === 1"
            (click)="pageChange.emit(currentPage() - 1)"
            aria-label="Previous page"
            class="px-3 py-1.5 rounded-lg text-sm font-medium border border-gray-200 bg-white text-gray-600 hover:bg-gray-50 disabled:opacity-40 disabled:cursor-not-allowed transition-colors">
            ‹
          </button>
          @for (p of pages(); track p) {
            <button
              (click)="pageChange.emit(p)"
              [attr.aria-label]="'Page ' + p"
              [attr.aria-current]="p === currentPage() ? 'page' : null"
              [class]="p === currentPage()
                ? 'px-3 py-1.5 rounded-lg text-sm font-medium bg-brand-blue text-white'
                : 'px-3 py-1.5 rounded-lg text-sm font-medium border border-gray-200 bg-white text-gray-600 hover:bg-gray-50 transition-colors'">
              {{ p }}
            </button>
          }
          <button
            [disabled]="currentPage() === totalPages()"
            (click)="pageChange.emit(currentPage() + 1)"
            aria-label="Next page"
            class="px-3 py-1.5 rounded-lg text-sm font-medium border border-gray-200 bg-white text-gray-600 hover:bg-gray-50 disabled:opacity-40 disabled:cursor-not-allowed transition-colors">
            ›
          </button>
        </div>
      </div>
    }
  `
})
export class PaginatorComponent {
  currentPage = input.required<number>();
  total = input.required<number>();
  pageSize = input<number>(10);
  pageChange = output<number>();

  totalPages = computed(() => Math.ceil(this.total() / this.pageSize()));
  rangeStart = computed(() => (this.currentPage() - 1) * this.pageSize() + 1);
  rangeEnd = computed(() => Math.min(this.currentPage() * this.pageSize(), this.total()));

  pages = computed(() => {
    const total = this.totalPages();
    const current = this.currentPage();
    const range: number[] = [];
    for (let i = Math.max(1, current - 2); i <= Math.min(total, current + 2); i++) {
      range.push(i);
    }
    return range;
  });
}
