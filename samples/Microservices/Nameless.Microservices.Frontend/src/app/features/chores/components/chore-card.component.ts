import { Component, input, output } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ChoreDto } from '../models/chore.model';

@Component({
  selector: 'app-chore-card',
  imports: [DatePipe],
  template: `
    <div [class]="'bg-white rounded-xl shadow-sm border-l-4 p-4 hover:shadow-md transition-shadow '
      + (chore().conclusionDate ? 'border-brand-green' : 'border-brand-amber')">
      <div class="flex items-start justify-between gap-3">
        <div class="min-w-0 flex-1">
          <div class="flex items-center gap-2 mb-1 flex-wrap">
            <h3 [class]="'font-semibold text-gray-800 truncate '
              + (chore().conclusionDate ? 'line-through text-gray-400' : '')">
              {{ chore().title }}
            </h3>
            @if (chore().conclusionDate) {
              <span class="shrink-0 inline-flex items-center text-xs font-semibold px-2 py-0.5 rounded-full bg-emerald-100 text-emerald-700">
                Done
              </span>
            }
          </div>
          @if (chore().description) {
            <p class="text-sm text-gray-500 line-clamp-2 mb-2">{{ chore().description }}</p>
          }
          <div class="flex flex-wrap gap-4 text-xs text-gray-400">
            @if (chore().dueDate) {
              <span>Due: {{ chore().dueDate | date:'mediumDate' }}</span>
            }
            @if (chore().conclusionDate) {
              <span>Completed: {{ chore().conclusionDate | date:'mediumDate' }}</span>
            }
          </div>
        </div>
        <div class="flex gap-1 shrink-0">
          @if (!chore().conclusionDate) {
            <button
              (click)="markDone.emit(chore().id)"
              title="Mark as done"
              aria-label="Mark chore as done"
              class="p-2 rounded-lg text-brand-green hover:bg-emerald-50 transition-colors">
              <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5" aria-hidden="true">
                <path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" />
              </svg>
            </button>
          }
          <button
            (click)="delete.emit(chore().id)"
            title="Delete chore"
            aria-label="Delete chore"
            class="p-2 rounded-lg text-red-400 hover:bg-red-50 transition-colors">
            <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2" aria-hidden="true">
              <path stroke-linecap="round" stroke-linejoin="round" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
            </svg>
          </button>
        </div>
      </div>
    </div>
  `
})
export class ChoreCardComponent {
  chore = input.required<ChoreDto>();
  markDone = output<string>();
  delete = output<string>();
}
