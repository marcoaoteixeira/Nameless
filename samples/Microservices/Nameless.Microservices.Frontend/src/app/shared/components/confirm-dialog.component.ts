import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-confirm-dialog',
  template: `
    <div class="fixed inset-0 z-50 flex items-center justify-center" role="dialog" aria-modal="true" [attr.aria-labelledby]="'confirm-title'">
      <div class="absolute inset-0 bg-black/40 backdrop-blur-sm" (click)="cancel.emit()"></div>
      <div class="relative bg-white rounded-2xl shadow-xl w-full max-w-sm mx-4 p-6">
        <h2 id="confirm-title" class="text-lg font-semibold text-gray-800 mb-2">{{ title() }}</h2>
        <p class="text-sm text-gray-500 mb-6">{{ message() }}</p>
        <div class="flex justify-end gap-3">
          <button
            (click)="cancel.emit()"
            class="px-4 py-2 rounded-lg text-sm font-medium border border-gray-200 text-gray-600 hover:bg-gray-50 transition-colors">
            Cancel
          </button>
          <button
            (click)="confirm.emit()"
            class="px-4 py-2 rounded-lg text-sm font-medium bg-red-500 text-white hover:bg-red-600 transition-colors">
            Delete
          </button>
        </div>
      </div>
    </div>
  `
})
export class ConfirmDialogComponent {
  title = input<string>('Are you sure?');
  message = input<string>('This action cannot be undone.');
  confirm = output<void>();
  cancel = output<void>();
}
