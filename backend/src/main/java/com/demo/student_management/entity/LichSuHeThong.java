package com.demo.student_management.entity;

import jakarta.persistence.*;
import lombok.*;
import java.time.LocalDateTime;

@Entity
@Table(name = "LICHSU_HETHONG")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
@EqualsAndHashCode(onlyExplicitlyIncluded = true)
public class LichSuHeThong {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "ID")
    @EqualsAndHashCode.Include
    private Long id;

    @Column(name = "THOI_GIAN", nullable = false)
    private LocalDateTime thoiGian;

    @Column(name = "NGUOI_THUC_HIEN", nullable = false, length = 255)
    private String nguoiThucHien;

    @Column(name = "HANH_DONG", nullable = false, length = 255)
    private String hanhDong;

    @Column(name = "CHI_TIET", columnDefinition = "TEXT")
    private String chiTiet;
}
