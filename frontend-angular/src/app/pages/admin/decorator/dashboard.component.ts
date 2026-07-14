import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { VistaPreviaComponent } from '../preview/decorator/vista-previa.component';
import { ProjectsAdminComponent } from '../../project/decorator/projects-admin.component';
import { AuthStateService } from '../../../services/auth-state.service';
import { AuthService } from '../../auth/api/auth.service';
import { ProfileService } from '../../profile/api/profile.service';
import { MyProfileDto } from '../../profile/types/profile.types';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { API_URL } from '../../../services-conf/api-config';
import { SkillDto } from '../types/dashboard.types';


type AdminSection = 'profile' | 'skills' | 'projects' | 'settings';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  templateUrl: '../ui/dashboard.component.html',
  styleUrls: ['../css/dashboard.component.css', '../css/dashboard-responsive.css'],
  imports: [FormsModule, CommonModule, VistaPreviaComponent, ProjectsAdminComponent]
})
export class AdminDashboardComponent implements OnInit {
  // Auth
  userName = signal('Andrés');
  userEmail = signal('');

  // Navegación
  activeSection = signal<AdminSection>('profile');
  sidebarOpen = signal(false);

  toggleSidebar(): void {
    this.sidebarOpen.set(!this.sidebarOpen());
  }

  selectSection(section: AdminSection): void {
    this.setSection(section);
    this.sidebarOpen.set(false);
  }
  sections: { key: AdminSection; label: string; icon: string }[] = [
    { key: 'profile', label: 'Perfil', icon: '👤' },
    { key: 'skills', label: 'Skills', icon: '⚡' },
    { key: 'projects', label: 'Projects', icon: '📂' },
    { key: 'settings', label: 'Settings', icon: '⚙️' }
  ];


  // Perfil
  profile = signal<MyProfileDto | null>(null);
  profileLoading = signal(true);
  profileError = signal<string | null>(null);
  profileSaving = signal(false);
  profileSaveMsg = signal<string | null>(null);
  photoUploading = signal(false);
  photoMsg = signal<string | null>(null);

  // Skills
  skills = signal<SkillDto[]>([]);
  skillsLoading = signal(true);
  skillsError = signal<string | null>(null);
  newSkill = signal({ name: '', skillType: 'Technology', description: '', isActive: true });
  skillImageFile = signal<File | null>(null);
  skillSaving = signal(false);
  skillSaveMsg = signal<string | null>(null);
  skillEditId = signal<number | null>(null);
  skillEditImageFile = signal<File | null>(null);
  skillEditRemoveImage = signal(false);
  skillTypes = ['Technology', 'Methodology', 'SoftSkill', 'Certification'];

  constructor(
    private authState: AuthStateService,
    private authService: AuthService,
    private router: Router,
    private http: HttpClient,
    public profileService: ProfileService
  ) {
    this.userName.set(this.authState.state.name ?? 'Andrés');
    this.userEmail.set(this.authState.state.email ?? '');
  }

  ngOnInit(): void {
    this.loadProfile();
  }

  // ================================================================
  // NAVEGACIÓN
  // ================================================================

  setSection(section: AdminSection): void {
    this.activeSection.set(section);
    if (section === 'skills' && this.skills().length === 0) {
      this.loadSkills();
    }
  }

  // ================================================================
  // PERFIL
  // ================================================================

  loadProfile(): void {
    this.profileLoading.set(true);
    this.profileError.set(null);
    this.profileService.getMyProfile().subscribe({
      next: (p) => { this.profile.set(p); this.profileLoading.set(false); },
      error: () => { this.profileError.set('No se pudo cargar el perfil.'); this.profileLoading.set(false); }
    });
  }

  saveProfile(): void {
    const p = this.profile();
    if (!p) return;
    this.profileSaving.set(true);
    this.profileSaveMsg.set(null);
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    this.http.patch(`${API_URL}/profile/me`, {
      name: p.name, lastName: p.lastName, phoneNumber: p.phoneNumber, country: p.country,
      city: p.city, state: p.state, zipCode: p.zipCode, title: p.title,
      summary: p.summary, available: p.available, availableText: p.availableText,
      education: p.education, educationStartYear: p.educationStartYear,
      educationEndYear: p.educationEndYear, linkedInUrl: p.linkedInUrl, gitHubUrl: p.gitHubUrl
    }, { headers }).subscribe({
      next: () => { this.profileSaving.set(false); this.profileSaveMsg.set('Perfil actualizado ✓'); setTimeout(() => this.profileSaveMsg.set(null), 3000); },
      error: () => { this.profileSaving.set(false); this.profileSaveMsg.set('Error al guardar.'); }
    });
  }

  onPhotoSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;
    this.photoUploading.set(true);
    this.photoMsg.set(null);
    const formData = new FormData();
    formData.append('file', input.files[0]);
    this.http.post<{ photoUrl: string }>(`${API_URL}/profile/photo`, formData).subscribe({
      next: (res) => {
        this.photoUploading.set(false); this.photoMsg.set('Foto actualizada ✓');
        const p = this.profile();
        // res.photoUrl ya es URL completa (GCS) o relativa (local), el mapper la transforma
        if (p) { p.photoUrl = res.photoUrl + '?t=' + Date.now(); this.profile.set({ ...p }); }
        setTimeout(() => this.photoMsg.set(null), 3000);
      },
      error: (err) => { this.photoUploading.set(false); this.photoMsg.set(err?.error?.message ?? 'Error al subir foto.'); }
    });
    input.value = '';
  }

  // ================================================================
  // SKILLS
  // ================================================================

  loadSkills(): void {
    this.skillsLoading.set(true);
    this.skillsError.set(null);
    this.http.get<SkillDto[]>(`${API_URL}/skill`).subscribe({
      next: (list) => { this.skills.set(list); this.skillsLoading.set(false); },
      error: () => { this.skillsError.set('No se pudieron cargar las skills.'); this.skillsLoading.set(false); }
    });
  }

  onSkillImageSelected(event: Event, editMode: boolean = false): void {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;
    const file = input.files[0];
    if (editMode) {
      this.skillEditImageFile.set(file);
      this.skillEditRemoveImage.set(false);
    } else {
      this.skillImageFile.set(file);
    }
    input.value = '';
  }

  createSkill(): void {
    const s = this.newSkill();
    if (!s.name.trim()) return;
    this.skillSaving.set(true);
    this.skillSaveMsg.set(null);
    const formData = new FormData();
    formData.append('name', s.name);
    formData.append('skillType', s.skillType);
    formData.append('description', s.description ?? '');
    formData.append('isActive', String(s.isActive));
    const img = this.skillImageFile();
    if (img) {
      formData.append('imageFile', img);
    }
    this.http.post<SkillDto>(`${API_URL}/skill`, formData).subscribe({
      next: (created) => {
        this.skills.set([...this.skills(), created]);
        this.newSkill.set({ name: '', skillType: 'Technology', description: '', isActive: true });
        this.skillImageFile.set(null);
        this.skillSaving.set(false);
      },
      error: (err) => { this.skillSaveMsg.set(err?.error?.message ?? 'Error al crear skill.'); this.skillSaving.set(false); setTimeout(() => this.skillSaveMsg.set(null), 3000); }
    });
  }

  toggleSkillActive(skill: SkillDto): void {
    const formData = new FormData();
    formData.append('name', skill.name);
    formData.append('skillType', skill.skillType);
    formData.append('description', skill.description ?? '');
    formData.append('isActive', String(!skill.isActive));
    this.http.patch<SkillDto>(`${API_URL}/skill/${skill.id}`, formData).subscribe({
      next: (updated) => { this.skills.set(this.skills().map(s => s.id === updated.id ? updated : s)); },
      error: () => { this.skillSaveMsg.set('Error al actualizar skill.'); setTimeout(() => this.skillSaveMsg.set(null), 3000); }
    });
  }

  editSkill(skill: SkillDto): void {
    const isOpening = skill.id !== this.skillEditId();
    this.skillEditId.set(isOpening ? skill.id : null);
    if (isOpening) {
      // Resetear estado de edición de imagen
      this.skillEditImageFile.set(null);
      this.skillEditRemoveImage.set(false);
    }
  }

  saveSkillEdit(skill: SkillDto): void {
    const formData = new FormData();
    formData.append('name', skill.name);
    formData.append('skillType', skill.skillType);
    formData.append('description', skill.description ?? '');
    formData.append('isActive', String(skill.isActive));
    const editImg = this.skillEditImageFile();
    if (editImg) {
      formData.append('imageFile', editImg);
    }
    if (this.skillEditRemoveImage()) {
      formData.append('removeImage', 'true');
    }
    this.http.patch<SkillDto>(`${API_URL}/skill/${skill.id}`, formData).subscribe({
      next: (updated) => {
        this.skills.set(this.skills().map(s => s.id === updated.id ? updated : s));
        this.skillEditId.set(null);
        this.skillEditImageFile.set(null);
        this.skillEditRemoveImage.set(false);
      },
      error: () => { this.skillSaveMsg.set('Error al guardar cambios.'); setTimeout(() => this.skillSaveMsg.set(null), 3000); }
    });
  }

  removeEditSkillImage(): void {
    this.skillEditImageFile.set(null);
    this.skillEditRemoveImage.set(true);
  }

  deleteSkill(skill: SkillDto): void {
    if (!confirm(`¿Eliminar "${skill.name}"?`)) return;
    this.http.delete(`${API_URL}/skill/${skill.id}`).subscribe({
      next: () => { this.skills.set(this.skills().filter(s => s.id !== skill.id)); },
      error: () => { this.skillSaveMsg.set('Error al eliminar skill.'); setTimeout(() => this.skillSaveMsg.set(null), 3000); }
    });
  }

  // ================================================================
  // LOGOUT
  // ================================================================

  goHome(): void {
    this.router.navigate(['/']);
  }

  logout(): void {
    this.authService.logout().subscribe({
      next: () => this.router.navigate(['/']),
      error: () => this.router.navigate(['/'])
    });
  }
}