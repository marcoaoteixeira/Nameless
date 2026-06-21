import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { ChoresService } from '../../core/services/chores.service';
import { ChoreDto } from './models/chore.model';
import { ChoreCardComponent } from './components/chore-card.component';
import { CreateChoreModalComponent } from './components/create-chore-modal/create-chore-modal.component';
import { ConfirmDialogComponent } from '../../shared/components/confirm-dialog.component';
import { PaginatorComponent } from '../../shared/components/paginator.component';

type DoneFilter = 'all' | 'done' | 'pending';

@Component({
  selector: 'app-chores-page',
  imports: [ChoreCardComponent, CreateChoreModalComponent, ConfirmDialogComponent, PaginatorComponent],
  templateUrl: './chores.page.html'
})
export class ChoresPage implements OnInit {
  private readonly choresService = inject(ChoresService);

  readonly PAGE_SIZE = 10;

  chores = signal<ChoreDto[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);

  searchTitle = signal('');
  filterDone = signal<DoneFilter>('all');
  currentPage = signal(1);

  showCreateModal = signal(false);
  deleteConfirmId = signal<string | null>(null);

  paginatedChores = computed(() => {
    const start = (this.currentPage() - 1) * this.PAGE_SIZE;
    return this.chores().slice(start, start + this.PAGE_SIZE);
  });

  ngOnInit() {
    this.loadChores();
  }

  loadChores() {
    this.loading.set(true);
    this.error.set(null);

    const query: { Title?: string; Done?: boolean } = {};
    if (this.searchTitle()) query.Title = this.searchTitle();
    if (this.filterDone() === 'done') query.Done = true;
    if (this.filterDone() === 'pending') query.Done = false;

    this.choresService.list(query).subscribe({
      next: chores => {
        this.chores.set(chores);
        this.currentPage.set(1);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load chores. Please try again.');
        this.loading.set(false);
      }
    });
  }

  onSearch() {
    this.loadChores();
  }

  onFilterChange(value: string) {
    this.filterDone.set(value as DoneFilter);
    this.loadChores();
  }

  onSearchInput(value: string) {
    this.searchTitle.set(value);
  }

  onChoreCreated() {
    this.showCreateModal.set(false);
    this.loadChores();
  }

  onMarkDone(id: string) {
    this.choresService.markDone(id).subscribe({
      next: () => this.loadChores(),
      error: () => this.error.set('Failed to mark chore as done.')
    });
  }

  onDeleteRequest(id: string) {
    this.deleteConfirmId.set(id);
  }

  onDeleteConfirm() {
    const id = this.deleteConfirmId();
    if (!id) return;
    this.deleteConfirmId.set(null);
    this.choresService.delete(id).subscribe({
      next: () => this.loadChores(),
      error: () => this.error.set('Failed to delete chore.')
    });
  }

  onPageChange(page: number) {
    this.currentPage.set(page);
  }
}
